using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApi.Core.Models
{
    public class Account
    {
        [JsonIgnore]
        public ICollection<Movie> Watchlist { get; set; } = new HashSet<Movie>();
        [JsonIgnore]
        public ICollection<Person> FavouritePeople { get; set; } = new HashSet<Person>();
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        [JsonIgnore]
        [NotMapped]
        public int? Age
        {
            get
            {
                if (Birthday == null) return null;

                DateTime zeroTime = new(1, 1, 1);

                DateTime a = (DateTime)Birthday;
                DateTime b = DateTime.Now;

                TimeSpan span = b - a;
                int years = (zeroTime + span).Year - 1;
                return years;
            }
        }

    }
}
