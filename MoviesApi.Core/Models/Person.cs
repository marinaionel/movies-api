using System.Collections.Generic;

#nullable disable

namespace MoviesApi.Core.Models
{
    public class Person
    {
        public Person()
        {
            DirectedMovies = new HashSet<Movie>();
            ActedInMovies = new HashSet<Movie>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public long? Birth { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public ICollection<Movie> DirectedMovies { get; set; }
        public ICollection<Movie> ActedInMovies { get; set; }
    }
}