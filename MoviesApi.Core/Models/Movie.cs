using MoviesApi.Core.Interfaces;

namespace MoviesApi.Core.Models
{
    public class Movie : ISearchable
    {
        public string SearchString { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
