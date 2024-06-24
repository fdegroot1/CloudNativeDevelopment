using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using PokemonWebApi.Repositories;
using PokemonWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PokemonWebApi.Unit.Tests.Services
{
    public class TeamServiceTests
    {
        [Fact]
        public async Task CreateTeamAsync_ValidTeam_CreatesTeam()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateTeam_Valid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                // Configure the mapper mock
                mapperMock.Setup(m => m.Map<Team>(It.IsAny<TeamDto>())).Returns((TeamDto dto) => new Team
                {
                    TeamName = dto.TeamName,
                    Pokemons = dto.Pokemons.Select(p => new Pokemon
                    {
                        PokemonName = p.PokemonName,
                        PokemonType = p.PokemonType,
                        Moves = p.Moves.Select(m => new Move
                        {
                            MoveName = m.MoveName,
                            MoveType = m.MoveType,
                            MovePower = m.MovePower
                        }).ToList()
                    }).ToList()
                });

                mapperMock.Setup(m => m.Map<TeamDto>(It.IsAny<Team>())).Returns((Team team) => new TeamDto
                {
                    TeamName = team.TeamName,
                    Pokemons = team.Pokemons.Select(p => new PokemonDto
                    {
                        PokemonName = p.PokemonName,
                        PokemonType = p.PokemonType,
                        Moves = p.Moves.Select(m => new MoveDto
                        {
                            MoveName = m.MoveName,
                            MoveType = m.MoveType,
                            MovePower = m.MovePower
                        }).ToList()
                    }).ToList()
                });

                pokeApiServiceMock.Setup(p => p.CanLearnMoveAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                var teamDto = new TeamDto
                {
                    TeamName = "Team A",
                    Pokemons = new List<PokemonDto>
                    {
                        new PokemonDto
                        {
                            PokemonName = "Pikachu",
                            PokemonType = "Electric",
                            Moves = new List<MoveDto>
                            {
                                new MoveDto { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 },
                                new MoveDto { MoveName = "Quick Attack", MoveType = "Normal", MovePower = 40 }
                            }
                        }
                    }
                };

                // Act
                var result = await teamService.CreateTeamAsync(teamDto);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(teamDto.TeamName, result.TeamName);
                Assert.Equal(teamDto.Pokemons.Count, result.Pokemons.Count);

                var createdTeam = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamName == teamDto.TeamName);

                Assert.NotNull(createdTeam);
                Assert.Equal(teamDto.TeamName, createdTeam.TeamName);
                Assert.Equal(teamDto.Pokemons.Count, createdTeam.Pokemons.Count);
            }
        }

        [Fact]
        public async Task GetTeamByIdAsync_ValidId_ReturnsTeam()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "GetTeamById_Valid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var team = new Team
                {
                    TeamName = "Team A",
                    Pokemons = new List<Pokemon>
                    {
                        new Pokemon
                        {
                            PokemonName = "Pikachu",
                            PokemonType = "Electric",
                            Moves = new List<Move> { new Move { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 } }
                        }
                    }
                };
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                mapperMock.Setup(m => m.Map<TeamDto>(It.IsAny<Team>())).Returns((Team team) => new TeamDto { TeamName = team.TeamName });

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await teamService.GetTeamByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Team A", result.TeamName);
            }
        }

        [Fact]
        public async Task GetTeamByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "GetTeamById_Invalid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await teamService.GetTeamByIdAsync(99);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllTeamsAsync_ReturnsAllTeams()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "GetAllTeams_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                context.Teams.Add(new Team { TeamName = "Team A" });
                context.Teams.Add(new Team { TeamName = "Team B" });
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                mapperMock.Setup(m => m.Map<IEnumerable<TeamDto>>(It.IsAny<IEnumerable<Team>>()))
                    .Returns((IEnumerable<Team> teams) => teams.Select(t => new TeamDto { TeamName = t.TeamName }));

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await teamService.GetAllTeamsAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.Contains(result, t => t.TeamName == "Team A");
                Assert.Contains(result, t => t.TeamName == "Team B");
            }
        }

        [Fact]
        public async Task GetTeamByNameAsync_ValidName_ReturnsTeam()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "GetTeamByName_Valid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var team = new Team { TeamName = "Team A" };
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                mapperMock.Setup(m => m.Map<TeamDto>(It.IsAny<Team>())).Returns((Team team) => new TeamDto { TeamName = team.TeamName });

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await teamService.GetTeamByNameAsync("Team A");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Team A", result.TeamName);
            }
        }

        [Fact]
        public async Task GetTeamByNameAsync_InvalidName_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "GetTeamByName_Invalid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await teamService.GetTeamByNameAsync("Invalid Team");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task DeleteTeamByNameAsync_ValidName_DeletesTeam()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteTeamByName_Valid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var team = new Team { TeamName = "Team A" };
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await teamService.DeleteTeamByNameAsync("Team A");

                // Assert
                Assert.True(result);

                var deletedTeam = await context.Teams.FirstOrDefaultAsync(t => t.TeamName == "Team A");
                Assert.Null(deletedTeam);
            }
        }

        [Fact]
        public async Task DeleteTeamByNameAsync_InvalidName_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteTeamByName_Invalid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await teamService.DeleteTeamByNameAsync("Invalid Team");

                // Assert
                Assert.False(result);
            }
        }
    }
}
