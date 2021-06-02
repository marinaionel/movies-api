using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.AzureFunctions;
using MoviesApi.ApiClient.TMDbApi;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using Country = MoviesApi.Core.Models.Country;
using Genre = MoviesApi.Core.Models.Genre;
using Movie = MoviesApi.Core.Models.Movie;

namespace MoviesApi.DataFillers
{
    public class MovieFiller
    {
        private YoutubeAzureFunctionClient _youtubeAzureFunctionClient;
        private TMDbApiClient _tmDbApiClient;
        public MovieFiller(YoutubeAzureFunctionClient getTrailerAzureFunctionClient, TMDbApiClient tmDbApiClient)
        {
            _youtubeAzureFunctionClient = getTrailerAzureFunctionClient;
            _tmDbApiClient = tmDbApiClient;
        }

        public async Task FillMovie(Movie fullMovie, MoviesContext moviesContext)
        {
            Movie trackedMovie = await moviesContext.Movies.Where(m => m.Id == fullMovie.Id)
                                                           .AsTracking()
                                                           .FirstOrDefaultAsync();
            if (trackedMovie == null) return;

            if (string.IsNullOrWhiteSpace(fullMovie.TrailerYoutubeVideoId))
                trackedMovie.TrailerYoutubeVideoId = await _youtubeAzureFunctionClient.GetTrailer($"{trackedMovie.Title} {trackedMovie.Year} trailer");

            TMDbLib.Objects.Movies.Movie tmdbMovie = await _tmDbApiClient.ApiClient.GetMovieAsync(fullMovie.IdString);
            if (tmdbMovie == null) return;

            if (fullMovie.PosterUrl is null or Constants.Unknown && tmdbMovie.PosterPath != null)
                trackedMovie.PosterUrl = $"https://image.tmdb.org/t/p/w500{tmdbMovie.PosterPath}";

            trackedMovie.ReleaseDate ??= tmdbMovie.ReleaseDate;
            trackedMovie.Runtime ??= $"{tmdbMovie.Runtime} min";
            trackedMovie.BoxOffice ??= $"${tmdbMovie.Revenue:#,0}";
            trackedMovie.Plot ??= tmdbMovie.Overview.Replace("\u200B", "");

            foreach (ProductionCountry country in tmdbMovie.ProductionCountries)
            {
                Country existingCountry = await moviesContext.Countries.Where(c => c.Name.ToLower() == country.Name.ToLower())
                                                                       .FirstOrDefaultAsync();
                if (existingCountry == null)
                {
                    Country newCountry = new() { Name = country.Name.Trim() };
                    await moviesContext.Countries.AddAsync(newCountry);
                    trackedMovie.Countries.Add(newCountry);
                }
                else if (fullMovie.Countries.All(l => l.Id != existingCountry.Id))
                    trackedMovie.Countries.Add(existingCountry);
            }

            foreach (SpokenLanguage language in tmdbMovie.SpokenLanguages)
            {
                Language existingLanguage = await moviesContext.Languages.Where(l => l.Name.ToLower() == language.Name.ToLower())
                                                                         .FirstOrDefaultAsync();
                if (existingLanguage == null)
                {
                    Language newLanguage = new() { Name = language.Name.Trim() };
                    await moviesContext.Languages.AddAsync(newLanguage);
                    trackedMovie.Languages.Add(newLanguage);
                }
                else if (fullMovie.Languages.All(l => l.Id != existingLanguage.Id))
                    trackedMovie.Languages.Add(existingLanguage);
            }

            foreach (TMDbLib.Objects.General.Genre genre in tmdbMovie.Genres)
            {
                Genre existingGenre = await moviesContext.Genres.Where(g => g.Name.Trim().ToLower() == genre.Name.Trim().ToLower())
                                                                .FirstOrDefaultAsync();

                if (existingGenre == null)
                {
                    Genre newGenre = new() { Name = genre.Name.Trim() };
                    await moviesContext.Genres.AddAsync(newGenre);
                    trackedMovie.Genres.Add(newGenre);
                }
                else if (fullMovie.Genres.All(g => g.Id != existingGenre.Id))
                    trackedMovie.Genres.Add(existingGenre);
            }
            await moviesContext.SaveChangesAsync();
        }
    }
}
