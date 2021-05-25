using Flurl.Http;
using MoviesApi.Common;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiClient.AzureFunctions
{
    public class YoutubeAzureFunctionClient
    {
        private const string ApiLink = "https://get-trailer-from-youtube-api.azurewebsites.net/api/get-trailer-from-youtube?code=zijLO/nCkZsXhsAcaXjwGRBagifTM7pGpU1Xcm85FRt9UqxE94vEWw==&search={0}";
        public async Task<string> GetTrailer(string search)
        {
            try
            {
                return await string.Format(ApiLink, search).GetStringAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Exception getting trailer with query {search} from get-trailer-from-youtube-api.azurewebsites.net", ex);
                return null;
            }
        }
    }
}
