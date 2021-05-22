using Flurl.Http;
using MoviesApi.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiClient.ImageApi
{
    public class QuantImageClient
    {
        private const string apiLink = "https://api.qwant.com/api/search/images?count=1&q={0}&t=images&safesearch=1&locale=en_US&uiv=4";
        public async Task<string> GetImageUrl(string query)
        {
            try
            {
                string json = await string.Format(apiLink, query).GetJsonAsync();
                if (json == null) return null;
                dynamic data = JObject.Parse(json);
                return data.data.result.items[0].thumbnail;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Exception making a request to {string.Format(apiLink, query)}", ex);
                return null;
            }
        }
    }
}
