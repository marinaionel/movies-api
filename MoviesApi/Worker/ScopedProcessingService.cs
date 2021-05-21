using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.AzureFunctions;
using MoviesApi.ApiClient.OMDbApi;
using MoviesApi.Common;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesApi.Worker
{
    internal class ScopedProcessingService : IScopedProcessingService
    {
        private const string Unknown = "N/A";
        private MoviesContext _moviesContext;
        private OMDBbServiceClient _oMDBbServiceClient;
        private GetTrailerClient _getTrailerClient;

        public ScopedProcessingService(MoviesContext moviesContext, OMDBbServiceClient oMDBbServiceClient, GetTrailerClient getTrailerClient)
        {
            _moviesContext = moviesContext;
            _oMDBbServiceClient = oMDBbServiceClient;
            _getTrailerClient = getTrailerClient;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                IQueryable<Movie> movies = _moviesContext.Movies.AsTracking()
                                                                .AsQueryable();

                foreach (Movie movie in movies)
                {
                    Core.Models.OMDb.Movie movieOmdb = await _oMDBbServiceClient.GetMovie(movie.IdString);
                    if (movieOmdb == null) continue;

                    movie.Plot = movieOmdb.Plot;
                    movie.PosterUrl = movieOmdb.Poster == "N/A" ? null : movieOmdb.Poster;
                    movie.Runtime = movieOmdb.Runtime;
                    movie.TrailerYoutubeVideoId = await _getTrailerClient.GetTrailer($"{movie.Title} {movie.Year} trailer");

                    Movie fullMovie = await _moviesContext.Movies.Where(m => m.Id == movie.Id)
                                                                 .Include(m => m.Genres)
                                                                 .Include(m => m.Languages)
                                                                 .AsNoTracking()
                                                                 .FirstOrDefaultAsync();

                    if (!string.IsNullOrWhiteSpace(movieOmdb.Language) && movieOmdb.Language != Unknown)
                    {
                        List<Language> languagesList = movieOmdb.Genre.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                                      .Select(l => new Language { Name = l.Trim() })
                                                                      .ToList();
                        foreach (Language language in languagesList)
                        {
                            Language existingLanguage = await _moviesContext.Languages.Where(l => l.Name.ToLower() == language.Name.ToLower())
                                                                                      .AsTracking()
                                                                                      .FirstOrDefaultAsync();
                            if (existingLanguage == null)
                            {
                                language.Movies.Add(movie);
                                await _moviesContext.Languages.AddAsync(language);
                                await _moviesContext.SaveChangesAsync();
                            }
                            else
                            {
                                if (!fullMovie.Languages.Any(l => l.Id == existingLanguage.Id))
                                {
                                    existingLanguage.Movies.Add(movie);
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
                                genre.Movies.Add(movie);
                                await _moviesContext.Genres.AddAsync(genre);
                                await _moviesContext.SaveChangesAsync();
                            }
                            else
                            {
                                if (!fullMovie.Languages.Any(l => l.Id == existingGenre.Id))
                                {
                                    existingGenre.Movies.Add(movie);
                                    await _moviesContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
                await _moviesContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error in ScopedProcessingService.DoWork", ex);
            }
        }
    }
}
