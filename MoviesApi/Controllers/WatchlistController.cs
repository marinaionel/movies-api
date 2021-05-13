using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        [HttpPost("AddToWatchList")]
        public void AddToWatchList(string movieId)
        {

        }
        [HttpPost("RemoveFromWatchList")]
        public void RemoveFromWatchList(string movieId)
        {

        }
    }
}
