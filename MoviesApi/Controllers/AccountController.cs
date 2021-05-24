using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Common;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [RequireHttps]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private MoviesContext _moviesContext;
        public AccountController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("GetReviews")]
        public async Task<ActionResult<IQueryable<Review>>> GetReviews(int userId, int max, int offset)
        {
            try
            {
                // return _moviesContext.Reviews.Where(r => r.AccountId == userId);
                return null;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting reviews for user {userId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("AddToWatchList")]
        public ActionResult AddToWatchList(string movieId)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error adding to watchlist", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("RemoveFromWatchList")]
        public ActionResult RemoveFromWatchList(string movieId)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error adding to watchlist", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
