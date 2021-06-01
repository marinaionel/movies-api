using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Helpers;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using MoviesApi.Requests;
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
        private string Domain => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        private MoviesContext _moviesContext;
        public ReviewController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet]
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

        [HttpPost]
        public async Task<ActionResult> PostReview(PostReviewRequest reviewRequest)
        {
            try
            {
                if (reviewRequest == null)
                    return BadRequest();

                if (!MovieHelper.ConvertIdToInt(reviewRequest.MovieId, out int movieIdAsInt))
                    return BadRequest();

                if (UserId == null)
                    return BadRequest("User not authenticated");

                Review review = new()
                {
                    AccountId = UserId,
                    MovieId = movieIdAsInt,
                    Rating = reviewRequest.Rating,
                    Title = reviewRequest.Title,
                    Text = reviewRequest.Text
                };

                if (!_moviesContext.Reviews.Any(r => r.MovieId == movieIdAsInt && r.AccountId == UserId))
                    _moviesContext.Reviews.AddAsync(review);
                else
                    _moviesContext.Reviews.Update(review);

                await _moviesContext.SaveChangesAsync();
                return Created($"{Domain}/api/Review?accountId={UserId}&movieId={movieIdAsInt}", review);
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error posting review {reviewRequest}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteReview(string movieId)
        {
            try
            {
                if (MovieHelper.ConvertIdToInt(movieId, out int idAsInt))
                    return BadRequest();

                Review review = await _moviesContext.Reviews.FirstOrDefaultAsync(r => r.AccountId == UserId && r.MovieId == idAsInt);

                if (review == null)
                    return BadRequest("Review does not exist");

                _moviesContext.Reviews.Remove(review);
                await _moviesContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error deleting review for movie {movieId} by user {UserId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
