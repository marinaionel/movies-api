using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Helpers;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private MoviesContext _moviesContext;
        public ReviewController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("GetReview")]
        public async Task<ActionResult<Review>> GetReview(int userId, string movieId)
        {
            try
            {
                if (MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                Review review = await _moviesContext.Reviews.FirstOrDefaultAsync(r => r.AccountId == userId && r.MovieId == idAsInt);
                return (ActionResult<Review>)review ?? NotFound();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting review for movie {movieId} by user {userId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("review")]
        public async Task<ActionResult> PostReview(Review review)
        {
            try
            {
                if (review == null) return BadRequest();
                if (review.MovieId == 0) return BadRequest();

                //TODO
                //if (review.AccountId==0) set the account id as the id of the current user

                _moviesContext.Reviews.Update(review);
                await _moviesContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error posting review {review}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("review")]
        public async Task<ActionResult> DeleteReview(int userId, string movieId)
        {
            try
            {
                if (MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                Review review = await _moviesContext.Reviews.FirstOrDefaultAsync(r => r.AccountId == userId && r.MovieId == idAsInt);

                if (review == null)
                    return BadRequest();

                _moviesContext.Reviews.Remove(review);
                await _moviesContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error deleting review for movie {movieId} by user {userId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
