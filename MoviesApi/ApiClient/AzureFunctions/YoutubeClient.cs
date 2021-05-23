using Flurl.Http;
using MoviesApi.Common;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiClient.AzureFunctions
{
    public class YoutubeClient
    {
        private const string apiLink = "https://get-trailer-from-youtube-api.azurewebsites.net/api/get-trailer-from-youtube?code=zijLO/nCkZsXhsAcaXjwGRBagifTM7pGpU1Xcm85FRt9UqxE94vEWw==&search={0}";
        public async Task<string> GetTrailer(string search)
        {
            try
            {
                return await string.Format(apiLink, search).GetStringAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Exception getting trailer with query {search} from get-trailer-from-youtube-api.azurewebsites.net", ex);
                return null;
            }
        }
    }
}
