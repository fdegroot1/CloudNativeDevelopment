using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonWebApi.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IPokemonDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPokeApiService _pokeApiService;

        public PokemonService(IPokemonDbContext context, IMapper mapper, IPokeApiService pokeApiService)
        {
            _context = context;
            _mapper = mapper;
            _pokeApiService = pokeApiService;
        }

        public async Task<bool> AddPokemonToTeamAsync(int teamId, PokemonDto pokemonDto)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
                return false;

            if (team.Pokemons.Count >= 6)
                return false;

            var newPokemon = _mapper.Map<Pokemon>(pokemonDto);
            team.Pokemons.Add(newPokemon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePokemonFromTeamAsync(int teamId, int pokemonId)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
                return false;

            var pokemonToRemove = team.Pokemons.FirstOrDefault(p => p.PokemonId == pokemonId);
            if (pokemonToRemove == null)
                return false;

            if (team.Pokemons.Count <= 1)
                return false;

            team.Pokemons.Remove(pokemonToRemove);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PokemonDto>> GetPokemonsInTeamAsync(int teamId)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
                return null;

            var pokemonDtos = _mapper.Map<IEnumerable<PokemonDto>>(team.Pokemons);
            return pokemonDtos;
        }
    }
}
