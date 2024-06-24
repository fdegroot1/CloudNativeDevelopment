using PokemonWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonWebApi.Services
{
    public interface IPokemonService
    {
        Task<bool> AddPokemonToTeamAsync(int teamId, PokemonDto pokemonDto);
        Task<bool> RemovePokemonFromTeamAsync(int teamId, int pokemonId);
        Task<IEnumerable<PokemonDto>> GetPokemonsInTeamAsync(int teamId);
       
    }
}
