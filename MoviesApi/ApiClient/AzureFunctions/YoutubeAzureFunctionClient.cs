using Flurl.Http;
using Microsoft.Extensions.Configuration;
using MoviesApi.Common;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiClient.AzureFunctions
{
    public class YoutubeAzureFunctionClient
    {
        private IConfiguration _configuration;

        public YoutubeAzureFunctionClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private const string ApiLink = "https://get-trailer-from-youtube-api.azurewebsites.net/api/get-trailer-from-youtube?code={0}&search={1}";
        public async Task<string> GetTrailer(string search)
        {
            try
            {
                return await string.Format(_configuration["GET-TRAILER-API-CODE"], ApiLink, search).GetStringAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Exception getting trailer with query {search} from get-trailer-from-youtube-api.azurewebsites.net", ex);
                return null;
            }
        }
    }
}
