using MoviesApi.Core.Interfaces;

namespace MoviesApi.Core.Models
{
    public class Movie : ISearchable
    {
        public string SearchString { get; set; }
        public string Id { get; set; }
    }
}
