using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonWebApi.Controllers;
using PokemonWebApi.Models;
using PokemonWebApi.Services;
using System.Threading.Tasks;
using Xunit;

namespace PokemonWebApi.Unit.Tests.Controllers
{
    public class PokemonControllerTests
    {
        [Fact]
        public async Task AddPokemonToTeam_ReturnsOk()
        {
            // Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            pokemonServiceMock.Setup(service => service.AddPokemonToTeamAsync(It.IsAny<int>(), It.IsAny<PokemonDto>()))
                              .ReturnsAsync(true); // Mocking successful operation

            var controller = new PokemonController(pokemonServiceMock.Object);

            // Act
            var result = await controller.AddPokemonToTeam(1, new PokemonDto { PokemonName = "Pikachu" });

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddPokemonToTeam_ReturnsBadRequest()
        {
            // Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            pokemonServiceMock.Setup(service => service.AddPokemonToTeamAsync(It.IsAny<int>(), It.IsAny<PokemonDto>()))
                              .ReturnsAsync(false); // Mocking operation failure

            var controller = new PokemonController(pokemonServiceMock.Object);

            // Act
            var result = await controller.AddPokemonToTeam(1, new PokemonDto { PokemonName = "Pikachu" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to add Pokémon to team.", badRequestResult.Value);
        }

        [Fact]
        public async Task RemovePokemonFromTeam_ReturnsOk()
        {
            // Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            pokemonServiceMock.Setup(service => service.RemovePokemonFromTeamAsync(It.IsAny<int>(), It.IsAny<int>()))
                              .ReturnsAsync(true); // Mocking successful operation

            var controller = new PokemonController(pokemonServiceMock.Object);

            // Act
            var result = await controller.RemovePokemonFromTeam(1, 1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RemovePokemonFromTeam_ReturnsNotFound()
        {
            // Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            pokemonServiceMock.Setup(service => service.RemovePokemonFromTeamAsync(It.IsAny<int>(), It.IsAny<int>()))
                              .ReturnsAsync(false); // Mocking operation failure

            var controller = new PokemonController(pokemonServiceMock.Object);

            // Act
            var result = await controller.RemovePokemonFromTeam(1, 1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pokemon or team not found.", notFoundResult.Value);
        }
    }
}
