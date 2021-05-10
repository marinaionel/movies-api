using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public List<ISearchable> Search(string q, Type type)
        {
            return null;
        }
    }
}
