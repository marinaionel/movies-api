using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.AzureFunctions;
using MoviesApi.ApiClient.ImageApi;
using MoviesApi.ApiClient.OMDbApi;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieOmdb = MoviesApi.Core.Models.OMDb.Movie;

namespace MoviesApi.DataFillers
{
    public class MovieFiller
    {
        private OMDBbClient _omdbClient;
        private YoutubeClient _youtubeClient;
        private QuantClient _quantClient;

        public MovieFiller(YoutubeClient getTrailerClient, OMDBbClient oMDBbServiceClient, QuantClient quantClient)
        {
            _omdbClient = oMDBbServiceClient;
            _youtubeClient = getTrailerClient;
            _quantClient = quantClient;
        }

        public async Task FillMoviePosterUrl(Movie movieParam, MoviesContext moviesContext)
        {
            try
            {
                Movie movie = await moviesContext.Movies.Where(m => m.Id == movieParam.Id)
                                                        .AsTracking()
                                                        .FirstOrDefaultAsync();
                if (movie == null) return;
                if (!string.IsNullOrEmpty(movie.PosterUrl)) return;
                string posterUrl = await _omdbClient.GetPosterUrl(movie.IdString);
                if (posterUrl == null)
                    posterUrl = await _quantClient.GetImageUrl($"{movie.Title} {movie.Year} movie poster");
                if (posterUrl == null) return;
                movie.PosterUrl = posterUrl;
                await moviesContext.SaveChangesAsync();
                movieParam.PosterUrl = posterUrl;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error in FillMoviePosterUrl movie {movieParam?.Id}", ex);
            }
        }

        public async Task FillMovie(Movie fullMovie, MoviesContext moviesContext)
        {
            Movie trackedMovie = await moviesContext.Movies.Where(m => m.Id == fullMovie.Id)
                                                           .AsTracking()
                                                           .FirstOrDefaultAsync();
            if (trackedMovie == null) return;

            if (string.IsNullOrWhiteSpace(fullMovie.TrailerYoutubeVideoId))
                trackedMovie.TrailerYoutubeVideoId = await _youtubeClient.GetTrailer($"{trackedMovie.Title} {trackedMovie.Year} trailer");

            MovieOmdb movieOmdb = await _omdbClient.GetMovie(fullMovie.IdString);
            if (movieOmdb == null) return;

            if (fullMovie.PosterUrl == null || fullMovie.PosterUrl == Constants.Unknown)
                if (!string.IsNullOrWhiteSpace(movieOmdb.Poster) && movieOmdb.Poster != Constants.Unknown)
                    trackedMovie.PosterUrl = movieOmdb.Poster;
                else
                {
                    string posterUrl = await _quantClient.GetImageUrl($"{fullMovie.Title} {fullMovie.Year} movie poster");
                    if (posterUrl != null)
                        trackedMovie.PosterUrl = posterUrl;
                }

            if (!string.IsNullOrWhiteSpace(movieOmdb.Released) && movieOmdb.Released != Constants.Unknown)
                trackedMovie.ReleaseDate = DateTime.Parse(movieOmdb.Released);

            trackedMovie.Runtime = movieOmdb.Runtime;
            trackedMovie.BoxOffice = movieOmdb.BoxOffice;
            trackedMovie.Plot = movieOmdb.Plot;

            await moviesContext.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(movieOmdb.Country) && movieOmdb.Country != Constants.Unknown)
            {
                List<Country> countriesList = movieOmdb.Country.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                               .Select(c => new Country { Name = c.Trim() })
                                                               .ToList();
                foreach (Country country in countriesList)
                {
                    Country existingCountry = await moviesContext.Countries.Where(c => c.Name.ToLower() == country.Name.ToLower())
                                                                           .FirstOrDefaultAsync();
                    if (existingCountry == null)
                    {
                        await moviesContext.Countries.AddAsync(country);
                        trackedMovie.Countries.Add(country);
                    }
                    else if (!fullMovie.Countries.Any(l => l.Id == existingCountry.Id))
                        trackedMovie.Countries.Add(existingCountry);
                }
                await moviesContext.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(movieOmdb.Language) && movieOmdb.Language != Constants.Unknown)
            {
                List<Language> languagesList = movieOmdb.Language.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                                 .Select(l => new Language { Name = l.Trim() })
                                                                 .ToList();
                foreach (Language language in languagesList)
                {
                    Language existingLanguage = await moviesContext.Languages.Where(l => l.Name.ToLower() == language.Name.ToLower())
                                                                             .FirstOrDefaultAsync();
                    if (existingLanguage == null)
                    {
                        await moviesContext.Languages.AddAsync(language);
                        trackedMovie.Languages.Add(language);
                    }
                    else if (!fullMovie.Languages.Any(l => l.Id == existingLanguage.Id))
                        trackedMovie.Languages.Add(existingLanguage);
                }
                await moviesContext.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(movieOmdb.Genre) && movieOmdb.Genre != Constants.Unknown)
            {
                List<Genre> genresList = movieOmdb.Genre.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(g => new Genre { Name = g.Trim() })
                                                        .ToList();
                foreach (Genre genre in genresList)
                {
                    Genre existingGenre = await moviesContext.Genres.Where(g => g.Name.Trim().ToLower() == genre.Name.Trim().ToLower())
                                                                    .FirstOrDefaultAsync();

                    if (existingGenre == null)
                    {
                        await moviesContext.Genres.AddAsync(genre);
                        trackedMovie.Genres.Add(genre);
                    }
                    else if (!fullMovie.Genres.Any(g => g.Id == existingGenre.Id))
                        trackedMovie.Genres.Add(existingGenre);
                }
                await moviesContext.SaveChangesAsync();
            }
        }
    }
}
