using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        [HttpGet]
        public Review GetReview(string id)
        {
            return null;
        }

        [HttpPut]
        public void PutReview(Review review)
        {

        }

        [HttpPost]
        public void PostReview(Review review)
        {

        }

        public void deleteReview(Review review)
        {

        }
    }
}
