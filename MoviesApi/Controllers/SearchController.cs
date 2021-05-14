using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Interfaces;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet("search")]
        public List<ISearchable> Search(string q, Core.Enums.Type type)
        {
            return null;
        }
    }
}
