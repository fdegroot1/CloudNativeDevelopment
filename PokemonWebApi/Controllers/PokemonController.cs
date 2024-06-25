using Microsoft.AspNetCore.Mvc;
using PokemonWebApi.Models;
using PokemonWebApi.Services;
using System.Threading.Tasks;

namespace PokemonWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        /// <summary>
        /// Adds a Pokemon to a specific team.
        /// </summary>
        /// <param name="teamId">The ID of the team.</param>
        /// <param name="pokemonDto">The Pokemon details.</param>
        /// <returns>Returns an ActionResult indicating the result of the operation.</returns>
        /// <response code="200">If the Pokemon was successfully added to the team.</response>
        /// <response code="400">If adding the Pokemon to the team failed.</response>
        [HttpPost("{teamId}")]
        public async Task<ActionResult> AddPokemonToTeam(int teamId, PokemonDto pokemonDto)
        {
            var success = await _pokemonService.AddPokemonToTeamAsync(teamId, pokemonDto);
            if (!success)
                return BadRequest("Failed to add Pokémon to team.");

            return Ok();
        }

        /// <summary>
        /// Removes a Pokemon from a specific team.
        /// </summary>
        /// <param name="teamId">The ID of the team.</param>
        /// <param name="pokemonId">The ID of the Pokemon to remove.</param>
        /// <returns>Returns an ActionResult indicating the result of the operation.</returns>
        /// <response code="200">If the Pokemon was successfully removed from the team.</response>
        /// <response code="404">If the Pokemon or team was not found.</response>
        [HttpDelete("{teamId}/{pokemonId}")]
        public async Task<ActionResult> RemovePokemonFromTeam(int teamId, int pokemonId)
        {
            var success = await _pokemonService.RemovePokemonFromTeamAsync(teamId, pokemonId);
            if (!success)
                return NotFound("Pokemon or team not found.");

            return Ok();
        }
    }
}
