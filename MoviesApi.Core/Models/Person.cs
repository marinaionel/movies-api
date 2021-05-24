using System.Collections.Generic;

#nullable disable

namespace MoviesApi.Core.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long? Birth { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public ICollection<Movie> DirectedMovies { get; set; } = new HashSet<Movie>();
        public ICollection<Movie> ActedInMovies { get; set; } = new HashSet<Movie>();
    }
}