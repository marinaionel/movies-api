using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.QuantApi;
using MoviesApi.ApiClient.TMDbApi;
using MoviesApi.Common;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DataFillers
{
    public class PersonFiller
    {
        private TMDbApiClient _tmDbApiClient;
        private QuantClient _quantClient;

        public PersonFiller(TMDbApiClient tmDbApiClient, QuantClient quantClient)
        {
            _tmDbApiClient = tmDbApiClient;
            _quantClient = quantClient;
        }

        public async Task FillPerson(Person fullPerson, MoviesContext moviesContext)
        {
            try
            {
                if (fullPerson == null) return;
                int? tmdbPersonId = (await _tmDbApiClient.ApiClient.SearchPersonAsync(fullPerson.Name))?.Results.FirstOrDefault()?.Id;
                if (tmdbPersonId == null) return;
                TMDbLib.Objects.People.Person tmdbPerson = await _tmDbApiClient.ApiClient.GetPersonAsync((int)tmdbPersonId);
                if (tmdbPerson == null) return;
                Person trackedPerson = await moviesContext.People.Where(p => p.Id == fullPerson.Id)
                                                                 .AsTracking()
                                                                 .FirstOrDefaultAsync();
                if (trackedPerson == null) return;
                trackedPerson.Birth ??= tmdbPerson.Birthday?.Year;
                trackedPerson.ImageUrl ??= tmdbPerson.Images?.Profiles?.FirstOrDefault()?.FilePath ?? await _quantClient.GetImageUrl(fullPerson.Name);
                if (!string.IsNullOrWhiteSpace(tmdbPerson.Biography))
                    trackedPerson.Description ??= tmdbPerson.Biography.Replace("\u200B", "");
                await moviesContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error in FillPerson", ex);
            }
        }
    }
}
