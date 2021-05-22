using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrewController : ControllerBase
    {
        private MoviesContext _moviesContext;
        public CrewController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetCrewMemberAsync(string id)
        {
            try
            {
                if (!int.TryParse(id, out int idAsInt))
                    return BadRequest();

                return await _moviesContext.People
                                           .Where(p => p.Id == idAsInt)
                                           .Include(p => p.ActedInMovies)
                                           .ThenInclude(m => m.Genres)
                                           .Include(p => p.DirectedMovies)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting crew member {id}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Person>>> GetCrewMembersAsync(int max, int offset)
        {
            try
            {
                return await _moviesContext.People
                                           .Include(p => p.ActedInMovies)
                                           .ThenInclude(m => m.Genres)
                                           .Include(p => p.DirectedMovies)
                                           .AsNoTracking()
                                           .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting crew members", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
