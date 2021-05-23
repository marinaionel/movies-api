using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Helpers;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using MoviesApi.DataFillers;
using System;
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
        private MovieFiller _movieFiller;

        public MovieController(MoviesContext moviesContext, MovieFiller movieFiller)
        {
            _moviesContext = moviesContext;
            _movieFiller = movieFiller;
        }

        [HttpGet("{movieId}")]
        public async Task<ActionResult<Movie>> GetMovieAsync(string movieId)
        {
            if (string.IsNullOrWhiteSpace(movieId))
                return BadRequest();

            try
            {
                if (!MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                Movie m = await _moviesContext.Movies.Where(m => m.Id == idAsInt)
                                                     .Include(m => m.Directors)
                                                     .Include(m => m.Actors)
                                                     .Include(m => m.Genres)
                                                     .Include(m => m.Languages)
                                                     .Include(m => m.Countries)
                                                     .AsNoTracking()
                                                     .FirstOrDefaultAsync();

                if (m == null) return NotFound();
                await _movieFiller.FillMovie(m, _moviesContext);
                m.Ratings = await _moviesContext.Ratings.Where(r => r.MovieId == m.Id).FirstOrDefaultAsync();
                return m;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting movie {movieId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Movie>>> GetMoviesAsync(int max, int offset)
        {
            try
            {
                List<Movie> movies = await _moviesContext.Movies.Include(m => m.Directors)
                                                           .Include(m => m.Actors)
                                                           .Include(m => m.Languages)
                                                           .Include(m => m.Genres)
                                                           .OrderBy(m => m.Id)
                                                           .Skip(offset)
                                                           .Take(max)
                                                           .AsNoTracking()
                                                           .ToListAsync();
                movies.ForEach(m => m.Ratings = _moviesContext.Ratings.Where(r => r.MovieId == m.Id).FirstOrDefault());
                return movies;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting movies", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews(string movieId, int max, int offset)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting reviews for movie {movieId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
