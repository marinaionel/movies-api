using MoviesApi.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MoviesApi.Core.Models
{
    public class Movie
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public long? Year { get; set; }
        [NotMapped]
        public string IdString => MovieHelper.ConvertIdToString(Id);

        public string BoxOffice { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string PosterUrl { get; set; }
        public string TrailerYoutubeVideoId { get; set; }
        public ICollection<Person> Actors { get; set; } = new HashSet<Person>();
        public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
        public ICollection<Person> Directors { get; set; } = new HashSet<Person>();
        public ICollection<Language> Languages { get; set; } = new HashSet<Language>();
        public ICollection<Country> Countries { get; set; } = new HashSet<Country>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        [JsonIgnore]
        public ICollection<Account> Watchers { get; set; } = new HashSet<Account>();
        [JsonIgnore]
        public ICollection<Chart> Charts { get; set; } = new HashSet<Chart>();
        public string Runtime { get; set; }
        public string Plot { get; set; }
        public TotalRatings TotalRatings { get; set; }
    }
}