using Microsoft.AspNetCore.Mvc;
using PokemonWebApi.Models;
using PokemonWebApi.Services;
using System.Threading.Tasks;

namespace PokemonWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoveController : ControllerBase
    {
        private readonly IMoveService _moveService;

        public MoveController(IMoveService moveService)
        {
            _moveService = moveService;
        }

        /// <summary>
        /// Adds a move to a specific Pokémon in a team.
        /// </summary>
        /// <param name="teamId">The ID of the team.</param>
        /// <param name="pokemonId">The ID of the Pokémon.</param>
        /// <param name="moveDto">The move details.</param>
        /// <returns>Returns an ActionResult indicating the result of the operation.</returns>
        /// <response code="200">If the move was successfully added.</response>
        /// <response code="404">If the team or Pokémon was not found.</response>
        [HttpPost("{teamId}/pokemon/{pokemonId}/moves")]
        public async Task<ActionResult> AddMoveToPokemon(int teamId, int pokemonId, MoveDto moveDto)
        {
            var success = await _moveService.AddMoveToPokemonAsync(teamId, pokemonId, moveDto);
            if (!success)
                return NotFound();

            return Ok();
        }

        /// <summary>
        /// Deletes a move from a specific Pokémon in a team.
        /// </summary>
        /// <param name="teamId">The ID of the team.</param>
        /// <param name="pokemonId">The ID of the Pokémon.</param>
        /// <param name="moveName">The name of the move to delete.</param>
        /// <returns>Returns an ActionResult indicating the result of the operation.</returns>
        /// <response code="204">If the move was successfully deleted.</response>
        /// <response code="404">If the team or Pokémon was not found.</response>
        [HttpDelete("{teamId}/pokemon/{pokemonId}/moves/{moveName}")]
        public async Task<ActionResult> DeleteMoveOfPokemon(int teamId, int pokemonId, string moveName)
        {
            var success = await _moveService.DeletePokemonMoveAsync(teamId, pokemonId, moveName);
            if (!success)
                return NotFound();

            return NoContent(); // 204 No Content response
        }
    }
}
