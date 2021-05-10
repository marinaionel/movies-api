using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Models;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        [HttpGet]
        public Movie GetMovie(string id)
        {
            return null;
        }

        [HttpGet]
        public IEnumerable<Movie> GetMovies(int max, int offset)
        {
            return null;
        }

        [HttpGet]
        public IEnumerable<Review> GetReviews(string movieId, int max, int offset)
        {
            return null;
        }
    }
}
