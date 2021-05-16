using MoviesApi.Core.Enums;
using System.Collections.Generic;

#nullable disable

namespace MoviesApi.Core.Model
{
    public class Person
    {
        public Person()
        {
            Directors = new HashSet<Director>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public long? Birth { get; set; }
        public string Description { get; set; }
        public List<CrewMemberType> Job { get; set; }

        public virtual ICollection<Director> Directors { get; set; }
    }
}