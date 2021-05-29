using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Helpers;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private string UserId => HttpContext.User.Claims.ToList().FirstOrDefault(x => x.Type == Constants.UserId)?.Value;

        private MoviesContext _moviesContext;
        public ReviewController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("GetReview")]
        public async Task<ActionResult<Review>> GetReview(string accountId, string movieId)
        {
            try
            {
                if (MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                Review review = await _moviesContext.Reviews.FirstOrDefaultAsync(r => r.AccountId == accountId && r.MovieId == idAsInt);
                return (ActionResult<Review>)review ?? NotFound();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting review for movie {movieId} by user {accountId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("review")]
        public async Task<ActionResult> PostReview(Review review)
        {
            try
            {
                if (review == null)
                    return BadRequest();

                if (review.MovieId == 0)
                    return BadRequest();

                if (UserId == null)
                    return BadRequest("User not authenticated");

                review.AccountId = UserId;

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
        public async Task<ActionResult> DeleteReview(string accountId, string movieId)
        {
            try
            {
                if (MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                Review review = await _moviesContext.Reviews.FirstOrDefaultAsync(r => r.AccountId == accountId && r.MovieId == idAsInt);

                if (review == null)
                    return BadRequest("Review does not exist");

                if (review.AccountId != UserId)
                    return Unauthorized();

                _moviesContext.Reviews.Remove(review);
                await _moviesContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error deleting review for movie {movieId} by user {accountId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
