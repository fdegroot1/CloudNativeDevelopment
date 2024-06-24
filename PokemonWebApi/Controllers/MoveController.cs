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

        [HttpPost("{teamId}/pokemon/{pokemonId}/moves")]
        public async Task<ActionResult> AddMoveToPokemon(int teamId, int pokemonId, MoveDto moveDto)
        {
            var success = await _moveService.AddMoveToPokemonAsync(teamId, pokemonId, moveDto);
            if (!success)
                return NotFound(); // or BadRequest, depending on your application logic

            return Ok();
        }

        [HttpDelete("{teamId}/pokemon/{pokemonId}/moves/{moveName}")]
        public async Task<ActionResult> DeleteMoveOfPokemon(int teamId, int pokemonId, string moveName)
        {
            var success = await _moveService.DeletePokemonMoveAsync(teamId, pokemonId, moveName);
            if (!success)
                return NotFound(); // or BadRequest, depending on your application logic

            return NoContent(); // 204 No Content response
        }
    }
}
