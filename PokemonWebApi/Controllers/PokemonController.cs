using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonWebApi.Models;

namespace PokemonWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonDbContext _pokemonDbContext;
        public PokemonController(PokemonDbContext pokemonDbContext)
        {
            _pokemonDbContext = pokemonDbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pokemon>> GetPokemons()
        {
            return _pokemonDbContext.Pokemons;
        }

        [HttpGet("{pokemonId:int}")]
        public async Task<ActionResult<Pokemon>> GetById(int pokemonId)
        {
            var pokemon = await _pokemonDbContext.Pokemons.FindAsync(pokemonId);
            return Ok(pokemon);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pokemon pokemon)
        {
            await _pokemonDbContext.Pokemons.AddAsync(pokemon);
            await _pokemonDbContext.SaveChangesAsync();
            return Ok(pokemon);
        }

        [HttpPut("{pokemonId:int}")]
        public async Task<IActionResult> Update(Pokemon pokemon)
        {
            _pokemonDbContext.Pokemons.Update(pokemon);
            await _pokemonDbContext.SaveChangesAsync();
            return Ok(pokemon);
        }

        [HttpDelete("{pokemonId:int}")]
        public async Task<IActionResult> Delete(int pokemonId)
        {
            var pokemon = await _pokemonDbContext.Pokemons.FindAsync(pokemonId);
            _pokemonDbContext.Pokemons.Remove(pokemon);
            await _pokemonDbContext.SaveChangesAsync();
            return Ok(pokemon);
        }


    }
}
