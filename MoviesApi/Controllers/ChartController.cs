using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using MoviesApi.DataFillers;
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
        private MovieFiller _movieFiller;
        public ChartController(MoviesContext moviesContext, MovieFiller movieFiller)
        {
            _moviesContext = moviesContext;
            _movieFiller = movieFiller;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Chart>>> GetChartsAsync(int max, int offset)
        {
            try
            {
                return await _moviesContext.Charts
                                            .Include(c => c.Movies)
                                            .ThenInclude(c => c.Genres)
                                            .Include(c => c.Movies)
                                            .ThenInclude(c => c.Directors)
                                            .Skip(offset)
                                            .Take(max)
                                            .AsNoTracking()
                                            .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting charts", ex);
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
