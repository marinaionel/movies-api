namespace MoviesApi.Requests
{
    public class PostReviewRequest
    {
        public string MovieId { get; set; }
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
