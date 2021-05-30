using Microsoft.Extensions.Configuration;
using TMDbLib.Client;

namespace MoviesApi.ApiClient.TMDbApi
{
    public class TMDbApiClient
    {
        public TMDbApiClient(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiClient = new(_configuration["API-KEY-TMDB"]);
        }

        private IConfiguration _configuration;
        public TMDbClient ApiClient { get; }
    }
}
