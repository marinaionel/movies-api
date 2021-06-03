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
                                                     .Include(m => m.TotalRatings)
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
        public ActionResult<ICollection<Movie>> GetMoviesAsync(int max, int offset)
        {
            try
            {
                HashSet<Movie> movies = _moviesContext.Movies.Include(m => m.Directors)
                                                           .Include(m => m.Actors)
                                                           .Include(m => m.Languages)
                                                           .Include(m => m.Genres)
                                                           .Include(m => m.Watchers)
                                                           .Include(m => m.TotalRatings)
                                                           .OrderBy(m => m.Id)
                                                           .Skip(offset)
                                                           .Take(max)
                                                           .AsNoTracking()
                                                           .ToHashSet();
                movies.ForEach(m => m.IsInMyWatchlist = m.Watchers.Any(mm => mm.Id == UserId));
                return movies;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error getting movies", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{movieId}/reviews")]
        public ActionResult<ICollection<Review>> GetMovieReviews(string movieId, int max = 10, int offset = 0)
        {
            try
            {
                if (!MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                HashSet<Review> reviews = _moviesContext.Reviews.Where(r => r.MovieId == idAsInt)
                    .Include(r => r.Movie)
                    .Include(r => r.Account)
                    .OrderBy(r => r.Rating)
                    .ThenBy(r => r.AccountId)
                    .Skip(offset)
                    .Take(max)
                    .ToHashSet();

                reviews.Where(r => r?.Account != null).ForEach(r =>
                {
                    r.Account.Birthday = null;
                    r.Account.Email = null;
                });

                return reviews;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error getting reviews for movie", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
