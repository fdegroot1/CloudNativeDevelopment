// /Services/TeamService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using PokemonWebApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonWebApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly IPokemonDbContext _context;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;
        private readonly IPokeApiService _pokeApiService;

        public TeamService(IPokemonDbContext context, IPokemonRepository pokemonRepository, IMapper mapper, IPokeApiService pokeApiService)
        {
            _context = context;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _pokeApiService = pokeApiService;
        }

        public async Task<TeamDto> CreateTeamAsync(TeamDto teamDto)
        {
            if (teamDto.Pokemons.Count > 6)
            {
                throw new ArgumentException("A team must have 6 Pokémon or less.");
            }

            var team = _mapper.Map<Team>(teamDto);

            foreach (var pokemon in team.Pokemons)
            {
                if (pokemon.Moves.Count > 4 || pokemon.Moves.Count == 0)
                {
                    throw new ArgumentException($"Pokémon {pokemon.PokemonName} must have between 1 and 4 moves.");
                }

                foreach (var move in pokemon.Moves)
                {
                    var canLearn = await _pokeApiService.CanLearnMoveAsync(pokemon.PokemonName.ToLower(), move.MoveName.ToLower());
                    if (!canLearn)
                    {
                        throw new ArgumentException($"Pokémon {pokemon.PokemonName} cannot learn the move {move.MoveName}.");
                    }
                }
            }

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<TeamDto> GetTeamByIdAsync(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .ThenInclude(p => p.Moves)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
                return null;

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            var teams = await _context.Teams
                .Include(t => t.Pokemons)
                .ThenInclude(p => p.Moves)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TeamDto>>(teams);
        }

        public async Task<TeamDto> GetTeamByNameAsync(string name)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .ThenInclude(p => p.Moves)
                .FirstOrDefaultAsync(t => t.TeamName.ToLower() == name.ToLower());

            if (team == null)
                return null;

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<bool> DeleteTeamByNameAsync(string name)
        {
            var team = await _context.Teams
                .Include(t => t.Pokemons)
                .ThenInclude(p => p.Moves)
                .FirstOrDefaultAsync(t => t.TeamName.ToLower() == name.ToLower());

            if (team == null)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
