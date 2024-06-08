using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PokemonWebApi.Models;
using Xunit;

public class PokemonControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PokemonControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPokemons_ReturnsListOfPokemons()
    {
        // Act
        var response = await _client.GetAsync("/api/pokemon");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Pikachu", responseString);
        Assert.Contains("Charmander", responseString);
    }

    [Fact]
    public async Task CreatePokemon_AddsPokemon()
    {
        // Arrange
        var newPokemon = new Pokemon { PokemonId = 3, PokemonName = "Bulbasaur" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pokemon", newPokemon);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdPokemon = JsonConvert.DeserializeObject<Pokemon>(await response.Content.ReadAsStringAsync());
        Assert.Equal("Bulbasaur", createdPokemon.PokemonName);
    }

    [Fact]
    public async Task UpdatePokemon_UpdatesPokemon()
    {
        // Arrange
        var updatedPokemon = new Pokemon { PokemonId = 1, PokemonName = "Raichu" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/pokemon/1", updatedPokemon);

        // Assert
        response.EnsureSuccessStatusCode();
        var returnedPokemon = JsonConvert.DeserializeObject<Pokemon>(await response.Content.ReadAsStringAsync());
        Assert.Equal("Raichu", returnedPokemon.PokemonName);
    }

    [Fact]
    public async Task DeletePokemon_DeletesPokemon()
    {
        // Act
        var response = await _client.DeleteAsync("/api/pokemon/2");

        // Assert
        response.EnsureSuccessStatusCode();
        var deletedPokemon = JsonConvert.DeserializeObject<Pokemon>(await response.Content.ReadAsStringAsync());
        Assert.Equal("Charmander", deletedPokemon.PokemonName);
    }
}
