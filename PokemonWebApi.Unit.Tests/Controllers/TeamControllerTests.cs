using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonWebApi.Controllers;
using PokemonWebApi.Models;
using PokemonWebApi.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PokemonWebApi.Unit.Tests.Controllers
{
    public class TeamsControllerTests
    {
        [Fact]
        public async Task CreateTeam_ReturnsCreatedAtAction()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.CreateTeamAsync(It.IsAny<TeamDto>()))
                           .ReturnsAsync(new TeamDto { TeamId = 1 }); // Mocking successful creation

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.CreateTeam(new TeamDto { TeamName = "Team A" });

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TeamsController.GetTeam), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task CreateTeam_ReturnsBadRequest()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.CreateTeamAsync(It.IsAny<TeamDto>()))
                           .ThrowsAsync(new ArgumentException("Invalid team data")); // Mocking ArgumentException

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.CreateTeam(new TeamDto { TeamName = "Team A" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid team data", badRequestResult.Value);
        }

        [Fact]
        public async Task GetTeam_ReturnsOk()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.GetTeamByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new TeamDto { TeamId = 1 }); // Mocking team retrieval

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetTeam(1);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTeam_ReturnsNotFound()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.GetTeamByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((TeamDto)null); // Mocking team not found

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetTeam(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllTeams_ReturnsOk()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.GetAllTeamsAsync())
                           .ReturnsAsync(new List<TeamDto> { new TeamDto { TeamId = 1 }, new TeamDto { TeamId = 2 } }); // Mocking team retrieval

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetAllTeams();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllTeams_ReturnsEmptyList()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.GetAllTeamsAsync())
                           .ReturnsAsync(new List<TeamDto>()); // Mocking empty list

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetAllTeams();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<TeamDto>>(okResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task DeleteTeamByName_ReturnsNoContent()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.DeleteTeamByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync(true); // Mocking successful deletion

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.DeleteTeamByName("Team A");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTeamByName_ReturnsNotFound()
        {
            // Arrange
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(service => service.DeleteTeamByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync(false); // Mocking deletion failure

            var mapperMock = new Mock<IMapper>(); // Mocking IMapper if needed

            var controller = new TeamsController(teamServiceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.DeleteTeamByName("Team A");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
