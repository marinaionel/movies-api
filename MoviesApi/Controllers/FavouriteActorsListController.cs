using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Models;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteActorsListController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Person>> GetFavouriteActorsList()
        {
            return null;
        }

        [HttpPost("AddToFavouriteList")]
        public ActionResult AddToFavouriteList(string actorId)
        {
            return null;
        }

        [HttpPost("RemoveFromFavouriteList")]
        public ActionResult RemoveFromFavouriteList(string actorId)
        {
            return null;
        }
    }
}
