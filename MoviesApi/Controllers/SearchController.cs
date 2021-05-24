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
        public ActionResult<List<object>> Search(string q, EntityType? type)
        {
            try
            {
                List<object> results = new();
                if (string.IsNullOrWhiteSpace(q))
                    return results;

                q = q.ToLower().Trim();

                if (type == null)
                {
                    results.AddRange(_moviesContext.Movies.Where(m => m.Title.ToLower().Contains(q)));
                    results.AddRange(_moviesContext.People.Where(p => p.Name.ToLower().Contains(q)));
                }
                else
                {
                    switch (type)
                    {
                        case EntityType.CrewMember:
                            results.AddRange(_moviesContext.People.Where(p => p.Name.ToLower().Contains(q)));
                            break;
                        case EntityType.Movie:
                            results.AddRange(_moviesContext.Movies.Where(m => m.Title.ToLower().Contains(q)));
                            break;
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error searching ", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
