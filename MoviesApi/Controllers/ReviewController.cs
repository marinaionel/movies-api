using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        [HttpGet("GetReview")]
        public Review GetReview(string id)
        {
            return null;
        }

        [HttpPut("review")]
        public void PutReview(Review review)
        {

        }

        [HttpPost("review")]
        public void PostReview(Review review)
        {

        }
        [HttpDelete("review")]
        public void deleteReview(Review review)
        {

        }
    }
}
