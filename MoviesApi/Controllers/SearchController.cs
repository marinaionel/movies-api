using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Interfaces;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet("s")]
        public List<ISearchable> Search(string q, Core.Enums.Type type)
        {
            return null;
        }
    }
}
