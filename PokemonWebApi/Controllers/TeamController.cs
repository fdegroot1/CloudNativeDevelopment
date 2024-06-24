using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PokemonWebApi.Models;
using PokemonWebApi.Services;

namespace PokemonWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamsController(ITeamService teamService, IMapper mapper)
        {
            _teamService = teamService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(TeamDto teamDto)
        {
            try
            {
                var teamDtoResult = await _teamService.CreateTeamAsync(teamDto);
                return CreatedAtAction(nameof(GetTeam), new { id = teamDtoResult.TeamId }, teamDtoResult);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            var teamDto = await _teamService.GetTeamByIdAsync(id);
            if (teamDto == null)
                return NotFound();

            return teamDto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllTeams()
        {
            var teamDtos = await _teamService.GetAllTeamsAsync();
            return Ok(teamDtos);
        }

        [HttpGet("byname/{name}")]
        public async Task<ActionResult<TeamDto>> GetTeamByName(string name)
        {
            var teamDto = await _teamService.GetTeamByNameAsync(name);
            if (teamDto == null)
                return NotFound();

            return teamDto;
        }

        [HttpDelete("byname/{name}")]
        public async Task<IActionResult> DeleteTeamByName(string name)
        {
            var success = await _teamService.DeleteTeamByNameAsync(name);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
