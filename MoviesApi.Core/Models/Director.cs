#nullable disable

namespace MoviesApi.Core.Model
{
    public partial class Director
    {
        public int MovieId { get; set; }
        public int PersonId { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual Person Person { get; set; }
    }
}