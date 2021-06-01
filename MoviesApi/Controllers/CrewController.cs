using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Extensions;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using MoviesApi.DataFillers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CrewController : ControllerBase
    {
        private MoviesContext _moviesContext;
        private PersonFiller _personFiller;
        private string UserId => HttpContext.User.Claims.ToList().FirstOrDefault(x => x.Type == Constants.UserId)?.Value;

        public CrewController(MoviesContext moviesContext, PersonFiller personFiller)
        {
            _moviesContext = moviesContext;
            _personFiller = personFiller;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Person>> GetCrewMemberAsync(int id)
        {
            try
            {
                Person person = await _moviesContext.People
                                           .Where(p => p.Id == id)
                                           .Include(p => p.ActedInMovies)
                                           .ThenInclude(m => m.Genres)
                                           .Include(p => p.DirectedMovies)
                                           .Include(p => p.Fans)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();
                if (person == null)
                    return NotFound();
                await _personFiller.FillPerson(person, _moviesContext);
                person.IsMyFavourite = person.Fans.Any(p => p.Id == UserId);
                person.ActedInMovies.ForEach(m => m.TotalRatings = _moviesContext.TotalRatings.FirstOrDefault(r => r.MovieId == m.Id));
                person.DirectedMovies.ForEach(m => m.TotalRatings = _moviesContext.TotalRatings.FirstOrDefault(r => r.MovieId == m.Id));
                return person;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting crew member {id}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("all")]
        public ActionResult<ICollection<Person>> GetCrewMembersAsync(int max = 100, int offset = 0)
        {
            try
            {
                HashSet<Person> people = _moviesContext.People
                                                       .Include(p => p.ActedInMovies)
                                                       .ThenInclude(m => m.Genres)
                                                       .Include(p => p.DirectedMovies)
                                                       .Include(p => p.Fans)
                                                       .OrderBy(p => p.Id)
                                                       .Skip(offset)
                                                       .Take(max)
                                                       .AsNoTracking()
                                                       .ToHashSet();
                people.ForEach(p => p.IsMyFavourite = p.Fans.Any(pp => pp.Id == UserId));
                return people;
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error getting crew members", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
