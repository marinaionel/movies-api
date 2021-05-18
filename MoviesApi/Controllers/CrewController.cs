using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Enums;
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
                id = id.Replace("nm", "");
                if (!int.TryParse(id, out int idAsInt))
                    return BadRequest();

                return await _moviesContext.People
                                           .Where(p => p.Id == idAsInt)
                                           .Include(p => p.ActedInMovies)
                                           .ThenInclude(m => m.Genres)
                                           .Include(p => p.Jobs)
                                           .Include(p => p.DirectedMovies)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error crew member {id}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Person>>> GetCrewMembersAsync(int max, int offset, CrewMemberType? type)
        {
            try
            {
                if (type == null)
                {
                    return await _moviesContext.People
                           .Include(p => p.ActedInMovies)
                           .ThenInclude(m => m.Genres)
                           .Include(p => p.Jobs)
                           .Include(p => p.DirectedMovies)
                           .AsNoTracking()
                           .ToListAsync();
                }
                else
                {
                    return await _moviesContext.People
                           .Include(p => p.ActedInMovies)
                           .ThenInclude(m => m.Genres)
                           .Include(p => p.Jobs)
                           .Include(p => p.DirectedMovies)
                           .Where(p => p.Jobs.Contains((CrewMemberType)type))
                           .AsNoTracking()
                           .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error crew members of type {type}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
