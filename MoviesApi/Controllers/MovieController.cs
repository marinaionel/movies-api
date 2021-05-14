using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Model;
using MoviesApi.Core.Models;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        public MovieController()
        {

        }

        [HttpGet("getMovie/{id}")]
        public Movie GetMovie(string id)
        {
            return new Movie { Id = id, SearchString = id };
        }

        [HttpGet("GetMovies")]
        public IEnumerable<Movie> GetMovies(int max, int offset)
        {
            return null;
        }

        [HttpGet("GetReviews")]
        public IEnumerable<Review> GetReviews(string movieId, int max, int offset)
        {
            return new List<Review>();
        }
    }
}
