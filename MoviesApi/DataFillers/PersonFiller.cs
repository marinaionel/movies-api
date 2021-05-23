using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.ImageApi;
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
        private QuantClient _quantImageClient;

        public PersonFiller(QuantClient quantImageClient)
        {
            _quantImageClient = quantImageClient;
        }

        public async Task FillPerson(Person fullPerson, MoviesContext moviesContext)
        {
            try
            {
                if (fullPerson == null) return;
                if (!string.IsNullOrWhiteSpace(fullPerson.ImageUrl)) return;
                string imageUrl = await _quantImageClient.GetImageUrl(fullPerson.Name);
                if (imageUrl == null) return;
                Person trackedPerson = await moviesContext.People.Where(p => p.Id == fullPerson.Id)
                                                                 .AsTracking()
                                                                 .FirstOrDefaultAsync();
                if (trackedPerson == null) return;
                trackedPerson.ImageUrl = imageUrl;
                await moviesContext.SaveChangesAsync();
                fullPerson.ImageUrl = imageUrl;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error in FillPerson", ex);
            }
        }
    }
}
