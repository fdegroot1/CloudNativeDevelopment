using System.Threading.Tasks;

namespace PokemonWebApi.Services
{
    public interface IPokeApiService
    {
        Task<bool> CanLearnMoveAsync(string pokemonName, string moveName);
    }
}
