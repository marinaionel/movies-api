using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.AzureFunctions;
using MoviesApi.ApiClient.OMDbApi;
using MoviesApi.Common;
using MoviesApi.Core.Helpers;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private MoviesContext _moviesContext;
        private GetTrailerClient _getTrailerClient;
        private OMDBbServiceClient _oMDBbServiceClient;

        public MovieController(MoviesContext moviesContext, GetTrailerClient getTrailerClient, OMDBbServiceClient oMDBbServiceClient)
        {
            _moviesContext = moviesContext;
            _getTrailerClient = getTrailerClient;
            _oMDBbServiceClient = oMDBbServiceClient;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovieAsync(string id)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                if (!MovieHelper.ConvertIdToInt(id, out int idAsInt))
                    return BadRequest();

                Movie m = await _moviesContext.Movies.Where(m => m.Id == idAsInt)
                                                     .Include(m => m.Directors)
                                                     .Include(m => m.Actors)
                                                     .Include(m => m.Genres)
                                                     .Include(m => m.Languages)
                                                     .Include(m => m.Countries)
                                                     .AsNoTracking()
                                                     .FirstOrDefaultAsync();
                if (m != null)
                    await FillMovie(m);

                if (m != null && string.IsNullOrWhiteSpace(m.TrailerYoutubeVideoId))
                {
                    Movie trackedMovie = await _moviesContext.Movies.Where(m1 => m1.Id == m.Id).AsTracking().FirstOrDefaultAsync();
                    trackedMovie.TrailerYoutubeVideoId = await _getTrailerClient.GetTrailer($"{m.Title} {m.Year} trailer");
                    await _moviesContext.SaveChangesAsync();
                }
                stopwatch.Stop();
                Log.Default.Info($"Get movie took {stopwatch.ElapsedMilliseconds} ms");
                return m == null ? NotFound() : m;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting movie {id}", ex);
                stopwatch.Stop();
                Log.Default.Info($"Get movie took {stopwatch.ElapsedMilliseconds} ms");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private const string Unknown = "N/A";
        private async Task FillMovie(Movie movie)
        {
            Core.Models.OMDb.Movie movieOmdb = await _oMDBbServiceClient.GetMovie(movie.IdString);
            if (movieOmdb == null) return;

            movie.Plot = movieOmdb.Plot;
            if (string.IsNullOrWhiteSpace(movieOmdb.Poster) && movieOmdb.Poster != Unknown)
                movie.PosterUrl = movieOmdb.Poster;
            movie.Runtime = movieOmdb.Runtime;
            movie.BoxOffice = movieOmdb.BoxOffice;

            if (!string.IsNullOrWhiteSpace(movieOmdb.Released) && movieOmdb.Released != Unknown)
                movie.ReleaseDate = DateTime.Parse(movieOmdb.Released);

            await _moviesContext.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(movieOmdb.Country) && movieOmdb.Country != Unknown)
            {
                List<Country> countriesList = movieOmdb.Country.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                               .Select(c => new Country { Name = c.Trim() })
                                                               .ToList();
                foreach (Country country in countriesList)
                {
                    Country existingCountry = await _moviesContext.Countries.Where(c => c.Name.ToLower() == country.Name.ToLower())
                                                                            .AsTracking()
                                                                            .FirstOrDefaultAsync();
                    if (existingCountry == null)
                    {
                        await _moviesContext.Countries.AddAsync(country);
                        movie.Countries.Add(country);
                        await _moviesContext.SaveChangesAsync();
                    }
                    else
                    {
                        if (!movie.Countries.Any(l => l.Id == existingCountry.Id))
                        {
                            movie.Countries.Add(existingCountry);
                            await _moviesContext.SaveChangesAsync();
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(movieOmdb.Language) && movieOmdb.Language != Unknown)
            {
                List<Language> languagesList = movieOmdb.Language.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                              .Select(l => new Language { Name = l.Trim() })
                                                              .ToList();
                foreach (Language language in languagesList)
                {
                    Language existingLanguage = await _moviesContext.Languages.Where(l => l.Name.ToLower() == language.Name.ToLower())
                                                                              .AsTracking()
                                                                              .FirstOrDefaultAsync();
                    if (existingLanguage == null)
                    {
                        await _moviesContext.Languages.AddAsync(language);
                        movie.Languages.Add(language);
                        await _moviesContext.SaveChangesAsync();
                    }
                    else
                    {
                        if (!movie.Languages.Any(l => l.Id == existingLanguage.Id))
                        {
                            movie.Languages.Add(existingLanguage);
                            await _moviesContext.SaveChangesAsync();
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(movieOmdb.Genre) && movieOmdb.Genre != Unknown)
            {
                List<Genre> genresList = movieOmdb.Genre.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(g => new Genre { Name = g.Trim() })
                                                        .ToList();
                foreach (Genre genre in genresList)
                {
                    Genre existingGenre = await _moviesContext.Genres.Where(g => g.Name.Trim().ToLower() == genre.Name.Trim().ToLower())
                                                                     .FirstOrDefaultAsync();

                    if (existingGenre == null)
                    {
                        await _moviesContext.Genres.AddAsync(genre);
                        movie.Genres.Add(genre);
                        await _moviesContext.SaveChangesAsync();
                    }
                    else
                    {
                        if (!movie.Genres.Any(g => g.Id == existingGenre.Id))
                        {
                            movie.Genres.Add(existingGenre);
                            await _moviesContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Movie>>> GetMoviesAsync(int max, int offset)
        {
            try
            {
                List<Movie> m = await _moviesContext.Movies.Include(m => m.Directors)
                                                           .Include(m => m.Actors)
                                                           .Include(m => m.Languages)
                                                           .Include(m => m.Genres)
                                                           .OrderBy(m => m.Id)
                                                           .Skip(offset)
                                                           .Take(max)
                                                           .AsNoTracking()
                                                           .ToListAsync();
                return m;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting movies", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("reviews")]
        public IEnumerable<Review> GetReviews(string movieId, int max, int offset)
        {
            return new List<Review>();
        }
    }
}
