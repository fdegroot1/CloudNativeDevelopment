
using Newtonsoft.Json;
using PokemonWebApi.Models;

namespace PokemonWebApi.Repositories
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly HttpClient _httpClient;

        public PokemonRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> CanLearnMoveAsync(string pokemonName, string moveName)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{pokemonName}");
            if (!response.IsSuccessStatusCode)
                return false;

            var pokemonData = JsonConvert.DeserializeObject<PokemonData>(await response.Content.ReadAsStringAsync());
            return pokemonData.moves.Any(m => m.move.name == moveName);
        }
    }
}
