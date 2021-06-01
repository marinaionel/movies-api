using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Extensions;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private string UserId => HttpContext.User.Claims.ToList().FirstOrDefault(x => x.Type == Constants.UserId)?.Value;
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
            try
            {
                if (!MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                Movie movie = await _moviesContext.Movies.Where(m => m.Id == idAsInt)
                                                     .Include(m => m.Directors)
                                                     .Include(m => m.Actors)
                                                     .Include(m => m.Genres)
                                                     .Include(m => m.Languages)
                                                     .Include(m => m.Countries)
                                                     .Include(m => m.Reviews)
                                                     .ThenInclude(m => m.Account)
                                                     .Include(m => m.Watchers)
                                                     .AsNoTracking()
                                                     .FirstOrDefaultAsync();
                if (movie == null)
                    return NotFound();

                movie.Reviews.Where(r => r?.Account != null).ForEach(r =>
                    {
                        r.Account.Birthday = null;
                        r.Account.Email = null;
                    });

                await _movieFiller.FillMovie(movie, _moviesContext);
                movie.TotalRatings = await _moviesContext.TotalRatings.Where(r => r.MovieId == movie.Id).FirstOrDefaultAsync();
                movie.IsInMyWatchlist = movie.Watchers.Any(w => w.Id == UserId);
                return movie;
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
                                                           .Include(m => m.Watchers)
                                                           .OrderBy(m => m.Id)
                                                           .Skip(offset)
                                                           .Take(max)
                                                           .AsNoTracking()
                                                           .ToListAsync();
                movies.ForEach(m =>
                {
                    m.TotalRatings = _moviesContext.TotalRatings.FirstOrDefault(r => r.MovieId == m.Id);
                    m.IsInMyWatchlist = m.Watchers.Any(mm => mm.Id == UserId);
                });
                return movies;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error getting movies", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
