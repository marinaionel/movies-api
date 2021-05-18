using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MoviesApi.Core.Models
{
    public class Chart
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string Route { get; set; }
        public string Name { get; set; }
        public ICollection<Movie> Movies { get; set; }

        public Chart()
        {
            Movies = new HashSet<Movie>();
        }
    }
}
