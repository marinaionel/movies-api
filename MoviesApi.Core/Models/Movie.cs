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
        }
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public long? Year { get; set; }
        [NotMapped]
        public string IdString { get => "tt" + Id; }
        [NotMapped]
        public string SearchString { get => string.Join(',', Title); }
        public string PosterUrl { get; set; }
        public ICollection<Person> Actors { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Person> Directors { get; set; }
        public string Runtime { get; set; }
        public string Plot { get; set; }
    }
}