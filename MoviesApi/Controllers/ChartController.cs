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
    public class ChartController : ControllerBase
    {
        private MoviesContext _moviesContext;
        public ChartController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("all")]
        public ActionResult<ICollection<Chart>> GetChartsAsync(int max = 100, int offset = 0)
        {
            try
            {
                return _moviesContext.Charts
                                     .Include(c => c.Movies)
                                     .ThenInclude(c => c.Genres)
                                     .Include(c => c.Movies)
                                     .ThenInclude(c => c.Directors)
                                     .Include(c => c.Movies)
                                     .ThenInclude(c => c.Ratings)
                                     .OrderBy(c => c.Id)
                                     .Skip(offset)
                                     .Take(max)
                                     .AsNoTracking()
                                     .ToHashSet();
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error getting charts", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{r}")]
        public async Task<ActionResult<Chart>> GetChart(string r)
        {
            try
            {
                Chart chart = await _moviesContext.Charts
                                                   .Where(c => c.Route == r)
                                                   .Include(c => c.Movies)
                                                   .ThenInclude(c => c.Genres)
                                                   .Include(c => c.Movies)
                                                   .ThenInclude(c => c.Directors)
                                                   .Include(c => c.Movies)
                                                   .ThenInclude(m => m.Ratings)
                                                   .AsNoTracking()
                                                   .FirstOrDefaultAsync();

                //chart.Movies.Distinct()
                //            .ForEach(m => _movieFiller.FillMoviePosterUrl(m, _moviesContext));

                return chart == null ? NotFound() : chart;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting chart {r}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
