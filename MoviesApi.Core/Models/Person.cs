using MoviesApi.Core.Enums;
using System.Collections.Generic;

#nullable disable

namespace MoviesApi.Core.Model
{
    public class Person
    {
        public Person()
        {
            DirectedMovies = new HashSet<Movie>();
            ActedInMovies = new HashSet<Movie>();
            Jobs = new HashSet<CrewMemberType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public long? Birth { get; set; }
        public string Description { get; set; }
        public ICollection<CrewMemberType> Jobs { get; set; }
        public ICollection<Movie> DirectedMovies { get; set; }
        public ICollection<Movie> ActedInMovies { get; set; }
    }
}