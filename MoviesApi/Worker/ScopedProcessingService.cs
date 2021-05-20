using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.AzureFunctions;
using MoviesApi.ApiClient.OMDbApi;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesApi.Worker
{
    internal class ScopedProcessingService : IScopedProcessingService
    {
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
            List<Movie> movies = await _moviesContext.Movies.AsTracking().ToListAsync();

            foreach (Movie movie in movies)
            {
                Core.Models.OMDb.Movie movieOmdb = await _oMDBbServiceClient.GetMovie(movie.IdString);
                if (movieOmdb == null) continue;

                movie.Plot = movieOmdb.Plot;
                movie.PosterUrl = movieOmdb.Poster == "N/A" ? null : movieOmdb.Poster;
                movie.Runtime = movieOmdb.Runtime;
                movie.TrailerYoutubeVideoId = await _getTrailerClient.GetTrailer($"{movie.Title} {movie.Year} trailer");

                if (!string.IsNullOrWhiteSpace(movieOmdb.Genre))
                {
                    List<Genre> genresList = movieOmdb.Genre.Split(",", System.StringSplitOptions.RemoveEmptyEntries).Select(g => new Genre { Name = g }).ToList();
                    foreach (Genre genre in genresList)
                    {
                        Genre existingGenre = await _moviesContext.Genres.Where(g => g.Name.Trim().ToLower() == genre.Name.Trim().ToLower())
                                                                         .FirstOrDefaultAsync();

                        if (existingGenre == null)
                        {
                            genre.Movies.Add(movie);
                            await _moviesContext.Genres.AddAsync(genre);
                            existingGenre = await _moviesContext.Genres.Where(g => g.Name.Trim().ToLower() == genre.Name.Trim().ToLower())
                                                                       .FirstOrDefaultAsync();
                        }
                        existingGenre = await _moviesContext.Genres.Where(g => g.Id == existingGenre.Id)
                                                                   .AsTracking()
                                                                   .FirstOrDefaultAsync();

                        existingGenre.Movies.Add(movie);
                        movie.Genres.Add(existingGenre);

                        await _moviesContext.SaveChangesAsync();
                    }
                }
                await _moviesContext.SaveChangesAsync();
            }
        }
    }
}
