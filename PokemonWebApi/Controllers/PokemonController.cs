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

        [HttpPost("{teamId}")]
        public async Task<ActionResult> AddPokemonToTeam(int teamId, PokemonDto pokemonDto)
        {
            var success = await _pokemonService.AddPokemonToTeamAsync(teamId, pokemonDto);
            if (!success)
                return BadRequest("Failed to add Pokémon to team.");

            return Ok();
        }

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
