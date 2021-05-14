#nullable disable

namespace MoviesApi.Core.Model
{
    public class Star
    {
        public int MovieId { get; set; }
        public int PersonId { get; set; }
        public virtual Movie Movie { get; set; }
    }
}