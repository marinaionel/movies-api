using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
        public ActionResult<ICollection<object>> Search(string q, EntityType type = EntityType.Movie, int max = 100, int offset = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return null;

                q = q.ToLower().Trim();

                return type switch
                {
                    EntityType.CrewMember => new ActionResult<ICollection<object>>(_moviesContext.People.Where(p => p.Name.ToLower().Contains(q))
                        .OrderBy(p => p.Id)
                        .Skip(offset)
                        .Take(max)
                        .Select(m => (object)m)
                        .ToHashSet()),
                    EntityType.Movie => new ActionResult<ICollection<object>>(_moviesContext.Movies.Where(m => m.Title.ToLower().Contains(q))
                        .OrderBy(m => m.Id)
                        .Skip(offset)
                        .Take(max)
                        .Select(m => (object)m)
                        .ToHashSet()),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error searching ", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
