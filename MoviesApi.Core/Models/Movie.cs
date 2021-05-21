using MoviesApi.Core.Helpers;
using MoviesApi.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MoviesApi.Core.Models
{
    public class Movie : ISearchable
    {
        public Movie()
        {
            Actors = new HashSet<Person>();
            Genres = new HashSet<Genre>();
            Directors = new HashSet<Person>();
            Charts = new HashSet<Chart>();
            Countries = new HashSet<Country>();
        }

        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public long? Year { get; set; }
        [NotMapped]
        public string IdString { get => MovieHelper.ConvertIdToString(Id); }
        [NotMapped]
        [JsonIgnore]
        public string SearchString { get => string.Join(',', Title); }
        public string BoxOffice { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string PosterUrl { get; set; }
        public string TrailerYoutubeVideoId { get; set; }
        public ICollection<Person> Actors { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Person> Directors { get; set; }
        public ICollection<Language> Languages { get; set; }
        public ICollection<Country> Countries { get; set; }
        [JsonIgnore]
        public ICollection<Chart> Charts { get; set; }
        public string Runtime { get; set; }
        public string Plot { get; set; }
    }
}