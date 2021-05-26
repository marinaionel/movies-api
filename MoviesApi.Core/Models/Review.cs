using Newtonsoft.Json;

namespace MoviesApi.Core.Models
{
    public class Review
    {
        public int MovieId { get; set; }
        [JsonIgnore]
        public Movie Movie { get; set; }
        public string AccountId { get; set; }
        public Account Account { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        private int _rating;
        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value switch
                {
                    > 10 => 10,
                    < 1 => 1,
                    _ => value
                };
            }
        }
    }
}
