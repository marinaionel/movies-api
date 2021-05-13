using Microsoft.AspNetCore.Mvc;
using MoviesApi.Core.Enums;
using MoviesApi.Core.Models;
using System.Collections.Generic;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrewController : ControllerBase
    {
        [HttpGet("GetCrewMember")]
        public CrewMember GetCrewMember(string id)
        {
            return null;
        }

        [HttpGet("GetCrewMembers")]
        public List<CrewMember> GetCrewMembers(int max, int offset, CrewMemberType type)
        {
            return null;
        }
    }
}
