using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private MoviesContext _moviesContext;
        public MovieController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("getMovie/{id}")]
        public async Task<ActionResult<Movie>> GetMovieAsync(string id)
        {
            id = id.Replace("tt", "");
            if (!int.TryParse(id, out int idAsString))
                return BadRequest();

            Movie m = await _moviesContext.Movies.Where(m => m.Id == idAsString)
                                                 .Include(m => m.Directors)
                                                 .Include(m => m.Actors)
                                                 .FirstOrDefaultAsync();
            return m == null ? NotFound() : m;
        }

        [HttpGet("GetMovies")]
        public async Task<IEnumerable<Movie>> GetMoviesAsync(int max, int offset)
        {
            IEnumerable<Movie> m = await _moviesContext.Movies.Skip(offset)
                                                              .Take(max)
                                                              .Include(m => m.Directors)
                                                              .Include(m => m.Actors)
                                                              .ToListAsync();
            return m;
        }

        [HttpGet("GetReviews")]
        public IEnumerable<Review> GetReviews(string movieId, int max, int offset)
        {
            return new List<Review>();
        }
    }
}
