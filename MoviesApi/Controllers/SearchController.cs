using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Enums;
using MoviesApi.Data;
using System.Collections.Generic;
using System.Linq;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private MoviesContext _moviesContext;
        public SearchController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("search")]
        public List<object> Search(string q, Core.Enums.EntityType? type)
        {
            List<object> results = new();
            if (string.IsNullOrWhiteSpace(q))
                return results;

            q = q.ToLower().Trim();

            if (type == null)
            {
                results.Add(_moviesContext.Movies.Where(m => m.Title.ToLower().Contains(q)));
                results.Add(_moviesContext.People.Where(p => p.Name.ToLower().Contains(q)));
            }
            else
            {
                switch (type)
                {
                    case EntityType.CrewMember:
                        results.Add(_moviesContext.People.Where(p => p.Name.ToLower().Contains(q)));
                        break;
                    case EntityType.Movie:
                        results.Add(_moviesContext.Movies.Where(m => m.Title.ToLower().Contains(q)));
                        break;
                }
            }

            return results;
        }
    }
}
