namespace MoviesApi.Core.Models
{
    public class Review
    {
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
                    > 5 => 5,
                    < 1 => 1,
                    _ => value
                };
            }
        }
    }
}
