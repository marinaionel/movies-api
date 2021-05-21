using System.Collections.Generic;

namespace MoviesApi.Core.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
}
