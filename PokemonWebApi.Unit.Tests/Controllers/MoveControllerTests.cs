using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonWebApi.Controllers;
using PokemonWebApi.Models;
using PokemonWebApi.Services;
using System.Threading.Tasks;
using Xunit;

namespace PokemonWebApi.Unit.Tests.Controllers
{
    public class MoveControllerTests
    {
        [Fact]
        public async Task AddMoveToPokemon_ReturnsOk()
        {
            // Arrange
            var moveServiceMock = new Mock<IMoveService>();
            moveServiceMock.Setup(service => service.AddMoveToPokemonAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<MoveDto>()))
                           .ReturnsAsync(true); // Mocking successful operation

            var controller = new MoveController(moveServiceMock.Object);

            // Act
            var result = await controller.AddMoveToPokemon(1, 1, new MoveDto { MoveName = "Move1" });

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddMoveToPokemon_ReturnsNotFound()
        {
            // Arrange
            var moveServiceMock = new Mock<IMoveService>();
            moveServiceMock.Setup(service => service.AddMoveToPokemonAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<MoveDto>()))
                           .ReturnsAsync(false); // Mocking operation failure

            var controller = new MoveController(moveServiceMock.Object);

            // Act
            var result = await controller.AddMoveToPokemon(1, 1, new MoveDto { MoveName = "Move1" });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteMoveOfPokemon_ReturnsNoContent()
        {
            // Arrange
            var moveServiceMock = new Mock<IMoveService>();
            moveServiceMock.Setup(service => service.DeletePokemonMoveAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                           .ReturnsAsync(true); // Mocking successful operation

            var controller = new MoveController(moveServiceMock.Object);

            // Act
            var result = await controller.DeleteMoveOfPokemon(1, 1, "Move1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMoveOfPokemon_ReturnsNotFound()
        {
            // Arrange
            var moveServiceMock = new Mock<IMoveService>();
            moveServiceMock.Setup(service => service.DeletePokemonMoveAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                           .ReturnsAsync(false); // Mocking operation failure

            var controller = new MoveController(moveServiceMock.Object);

            // Act
            var result = await controller.DeleteMoveOfPokemon(1, 1, "Move1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
