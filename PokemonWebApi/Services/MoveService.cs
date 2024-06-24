using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using PokemonWebApi.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonWebApi.Services
{
    public class MoveService : IMoveService
    {
        private readonly IPokemonDbContext _context;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;
        private readonly IPokeApiService _pokeApiService;

        public MoveService(IPokemonDbContext context, IPokemonRepository pokemonRepository, IMapper mapper, IPokeApiService pokeApiService)
        {
            _context = context;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _pokeApiService = pokeApiService;
        }

        public async Task<bool> DeletePokemonMoveAsync(int teamId, int pokemonId, string move)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .ThenInclude(p => p.Moves)
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
                return false;

            var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == pokemonId);
            if (pokemon == null)
                return false;

            var moveEntity = pokemon.Moves.FirstOrDefault(m => m.MoveName.ToLower() == move.ToLower());
            if (moveEntity == null)
                return false;

            pokemon.Moves.Remove(moveEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMoveToPokemonAsync(int teamId, int pokemonId, MoveDto move)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .ThenInclude(p => p.Moves)
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
                return false;

            var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == pokemonId);
            if (pokemon == null)
                return false;

            if (pokemon.Moves.Count >= 4)
                return false;

            var canLearn = await _pokeApiService.CanLearnMoveAsync(pokemon.PokemonName.ToLower(), move.MoveName.ToLower());
            if (!canLearn)
                return false;

            Console.WriteLine(pokemon.Moves.Count);

            var newMove = _mapper.Map<Move>(move);

            pokemon.Moves.Add(newMove);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
