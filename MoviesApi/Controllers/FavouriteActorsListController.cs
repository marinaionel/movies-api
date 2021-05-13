using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteActorsListController : ControllerBase
    {
        [HttpPost("AddToFavouriteList")]
        public void AddToFavouriteList(string actorId)
        {

        }

        [HttpPost("RemoveFromFavouriteList")]
        public void RemoveFromFavouriteList(string actorId)
        {

        }
    }
}
