#nullable disable

namespace MoviesApi.Core.Models
{
    public class TotalRatings
    {
        public int? MovieId { get; set; }
        public float? AverageRating { get; set; }
        public int? Votes { get; set; }
        public virtual Movie Movie { get; set; }
    }
}