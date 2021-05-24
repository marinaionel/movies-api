using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Common;
using MoviesApi.Core.Enums;
using MoviesApi.Data;
using System;
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

        [HttpGet]
        public ActionResult<HashSet<object>> Search(string q, EntityType type = EntityType.Movie, int max = 100, int offset = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return null;

                q = q.ToLower().Trim();

                switch (type)
                {
                    case EntityType.CrewMember:
                        return _moviesContext.People.Where(p => p.Name.ToLower().Contains(q))
                                                      .OrderBy(p => p.Id)
                                                      .Skip(offset)
                                                      .Take(max)
                                                      .Select(m => (object)m)
                                                      .ToHashSet();
                    case EntityType.Movie:
                        return _moviesContext.Movies.Where(m => m.Title.ToLower().Contains(q))
                                                      .OrderBy(m => m.Id)
                                                      .Skip(offset)
                                                      .Take(max)
                                                      .Select(m => (object)m)
                                                      .ToHashSet();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error searching ", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
