#nullable disable

namespace MoviesApi.Core.Models
{
    public class Rating
    {
        public int? MovieId { get; set; }
        public float? AverageRating { get; set; }
        public int? Votes { get; set; }
        public virtual Movie Movie { get; set; }
    }
}