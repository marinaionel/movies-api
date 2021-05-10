using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Models;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public List<Review> GetReviews(string userId, int max, int offset)
        {
            return null;
        }
    }
}
