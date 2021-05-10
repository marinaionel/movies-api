using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteActorsListController : ControllerBase
    {
        public void AddToFavouriteList(string actorId)
        {

        }

        public void RemoveFromFavouriteList(string actorId)
        {

        }
    }
}
