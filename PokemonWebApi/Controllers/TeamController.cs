using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using PokemonWebApi.Models;
using Microsoft.EntityFrameworkCore;
using PokemonWebApi.Data;
using PokemonWebApi.Repositories;

namespace PokemonWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly IPokemonDbContext _context;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;

        public TeamsController(IPokemonDbContext context, IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _context = context;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(TeamDto teamDto)
        {
            if (teamDto.Pokemons.Count != 6)
            {
                return BadRequest("A team must have exactly 6 Pokémon.");
            }

            var team = _mapper.Map<Team>(teamDto);

            foreach (var pokemon in team.Pokemons)
            {
                if (pokemon.Moves.Count != 4)
                {
                    return BadRequest($"Pokémon {pokemon.PokemonName} must have exactly 4 moves.");
                }

                foreach (var move in pokemon.Moves)
                {
                    var canLearn = await _pokemonRepository.CanLearnMoveAsync(pokemon.PokemonName.ToLower(), move.MoveName.ToLower());
                    if (!canLearn)
                    {
                        return BadRequest($"Pokémon {pokemon.PokemonName} cannot learn the move {move.MoveName}.");
                    }
                }
            }

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            var teamDtoResult = _mapper.Map<TeamDto>(team);

            return CreatedAtAction(nameof(GetTeam), new { id = teamDtoResult.TeamId }, teamDtoResult);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .ThenInclude(p => p.Moves)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
                return NotFound();

            var teamDto = _mapper.Map<TeamDto>(team);
            return teamDto;
        }
    }
}
