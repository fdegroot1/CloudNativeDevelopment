
using Newtonsoft.Json;
using PokemonWebApi.Models;
using System.Net.Http;

namespace PokemonWebApi.Services
{
    public class PokeApiService : IPokeApiService
    {
        private readonly HttpClient _httpClient;
        public PokeApiService(HttpClient httpClient) 
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
