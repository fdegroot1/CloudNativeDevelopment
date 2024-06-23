namespace PokemonWebApi.Repositories
{
    public interface IPokemonRepository
    {
        Task<bool> CanLearnMoveAsync(string pokemonName, string moveName);
    }
}
