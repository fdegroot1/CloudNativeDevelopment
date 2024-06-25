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

        /// <summary>
        /// Creates a new team.
        /// </summary>
        /// <param name="teamDto">The team details.</param>
        /// <returns>Returns the newly created team.</returns>
        /// <response code="201">Returns the newly created team.</response>
        /// <response code="400">If there is a validation error or the team cannot be created.</response>
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

        /// <summary>
        /// Retrieves a specific team by its ID.
        /// </summary>
        /// <param name="id">The ID of the team to retrieve.</param>
        /// <returns>Returns the requested team.</returns>
        /// <response code="200">Returns the requested team.</response>
        /// <response code="404">If the team with the specified ID is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            var teamDto = await _teamService.GetTeamByIdAsync(id);
            if (teamDto == null)
                return NotFound();

            return Ok(teamDto);
        }

        /// <summary>
        /// Retrieves all teams.
        /// </summary>
        /// <returns>Returns a list of all teams.</returns>
        /// <response code="200">Returns a list of all teams.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllTeams()
        {
            var teamDtos = await _teamService.GetAllTeamsAsync();
            return Ok(teamDtos);
        }

        /// <summary>
        /// Retrieves a specific team by its name.
        /// </summary>
        /// <param name="name">The name of the team to retrieve.</param>
        /// <returns>Returns the requested team.</returns>
        /// <response code="200">Returns the requested team.</response>
        /// <response code="404">If the team with the specified name is not found.</response>
        [HttpGet("byname/{name}")]
        public async Task<ActionResult<TeamDto>> GetTeamByName(string name)
        {
            var teamDto = await _teamService.GetTeamByNameAsync(name);
            if (teamDto == null)
                return NotFound();

            return teamDto;
        }

        /// <summary>
        /// Deletes a team by its name.
        /// </summary>
        /// <param name="name">The name of the team to delete.</param>
        /// <returns>Returns No Content if the team was successfully deleted.</returns>
        /// <response code="204">If the team was successfully deleted.</response>
        /// <response code="404">If the team with the specified name is not found.</response>
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
