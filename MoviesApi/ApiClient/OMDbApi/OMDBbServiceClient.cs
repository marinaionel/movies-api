﻿using Flurl.Http;
using MoviesApi.Common;
using MoviesApi.Core.Models.OMDb;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiClient.OMDbApi
{
    public class OMDBbServiceClient
    {
        private const string apiLink = "http://www.omdbapi.com/?i={0}&apikey=8b151be9&plot=full";

        public async Task<Movie> GetMovie(string id)
        {
            try
            {
                return await string.Format(apiLink, id).GetJsonAsync<Movie>();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Exception getting movie with id {id} from OMDb API", ex);
                return null;
            }
        }
    }
}
