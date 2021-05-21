using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.ApiClient.AzureFunctions;
using MoviesApi.Common;
using MoviesApi.Core.Helpers;
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
    public class MovieController : ControllerBase
    {
        private MoviesContext _moviesContext;
        private GetTrailerClient _getTrailerClient;

        public MovieController(MoviesContext moviesContext, GetTrailerClient getTrailerClient)
        {
            _moviesContext = moviesContext;
            _getTrailerClient = getTrailerClient;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovieAsync(string id)
        {
            try
            {
                if (!MovieHelper.ConvertIdToInt(id, out int idAsInt))
                    return BadRequest();

                Movie m = await _moviesContext.Movies.Where(m => m.Id == idAsInt)
                                                     .Include(m => m.Directors)
                                                     .Include(m => m.Actors)
                                                     .Include(m => m.Genres)
                                                     .Include(l => l.Languages)
                                                     .AsNoTracking()
                                                     .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(m.TrailerYoutubeVideoId))
                {
                    m.TrailerYoutubeVideoId = await _getTrailerClient.GetTrailer($"{m.Title} {m.Year} trailer");
                    _moviesContext.Update(m);
                    await _moviesContext.SaveChangesAsync();
                }
                return m == null ? NotFound() : m;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting movie {id}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Movie>>> GetMoviesAsync(int max, int offset)
        {
            try
            {
                List<Movie> m = await _moviesContext.Movies.Include(m => m.Directors)
                                                           .Include(m => m.Actors)
                                                           .Include(m => m.Languages)
                                                           .Include(m => m.Genres)
                                                           .OrderBy(m => m.Id)
                                                           .Skip(offset)
                                                           .Take(max)
                                                           .AsNoTracking()
                                                           .ToListAsync();
                return m;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting movies", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("reviews")]
        public IEnumerable<Review> GetReviews(string movieId, int max, int offset)
        {
            return new List<Review>();
        }
    }
}
