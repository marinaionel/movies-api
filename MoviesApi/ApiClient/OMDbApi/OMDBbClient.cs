﻿using Flurl.Http;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Models.OMDb;
using System;
using System.Threading.Tasks;

namespace MoviesApi.ApiClient.OMDbApi
{
    public class OMDBbClient
    {
        private const string apiLink = "http://www.omdbapi.com/?i={0}&apikey=8b151be9&plot=full";

        public async Task<Movie> GetMovie(string movieId)
        {
            try
            {
                return await string.Format(apiLink, movieId).GetJsonAsync<Movie>();
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
                string poster = (await string.Format(apiLink, movieId).GetJsonAsync<Movie>())?.Poster;
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
