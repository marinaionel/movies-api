using Flurl.Http;
using Microsoft.Extensions.Configuration;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Models.OMDb;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiClient.OMDbApi
{
    //not needed anymore as OMDb API was replaced by TMDb
    [Obsolete]
    public class OMDBbClient
    {
        private IConfiguration _configuration;
        public OMDBbClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private const string ApiLink = "http://www.omdbapi.com/?i={0}&apikey={1}&plot=full";

        public async Task<Movie> GetMovie(string movieId)
        {
            try
            {
                return await string.Format(ApiLink, _configuration["API-KEY-OMDB"], movieId).GetJsonAsync<Movie>();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Exception getting movie with id {movieId} from OMDb API", ex);
                return null;
            }
        }

        public async Task<string> GetPosterUrl(string movieId)
        {
            try
            {
                string poster = (await string.Format(ApiLink, _configuration["API-KEY-OMDB"], movieId).GetJsonAsync<Movie>())?.Poster;
                return poster == Constants.Unknown ? null : poster;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Exception getting poster url for movie with id {movieId} from OMDb API", ex);
                return null;
            }
        }
    }
}
