using MoviesApi.Core.Helpers;
using MoviesApi.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public string PosterUrl { get; set; }
        public string TrailerUrl { get; set; }
        public ICollection<Person> Actors { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Person> Directors { get; set; }
        [JsonIgnore]
        public ICollection<Chart> Charts { get; set; }
        public string Runtime { get; set; }
        public string Plot { get; set; }
    }
}