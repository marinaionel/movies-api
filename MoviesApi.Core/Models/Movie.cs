using MoviesApi.Core.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApi.Core.Models
{
    [Table("moviesfile.movies")]
    public class Movie : ISearchable
    {
        public string SearchString { get; set; }
        public string Id { get; set; }
    }
}
