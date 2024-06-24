using System.Threading.Tasks;

namespace PokemonWebApi.Services
{
    public interface IMoveService
    {
        Task<bool> DeletePokemonMoveAsync(int teamId, int pokemonId, string move);
        Task<bool> AddMoveToPokemonAsync(int teamId, int pokemonId, MoveDto move);
    }
}
