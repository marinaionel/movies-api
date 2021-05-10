using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Models;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        public List<Chart> GetCharts(int max, int offset, string reference)
        {
            return null;
        }

        public List<Chart> GetChart(string routeString)
        {
            return null;
        }
    }
}
