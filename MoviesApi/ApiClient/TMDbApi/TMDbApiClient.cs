using TMDbLib.Client;

namespace MoviesApi.ApiClient.TMDbApi
{
    public class TMDbApiClient
    {
        private const string ApiKey = "1dd138f66be96e0446edbd32265527ab";
        public TMDbClient ApiClient { get; }

        public TMDbApiClient()
        {
            ApiClient = new(ApiKey);
        }
    }
}
