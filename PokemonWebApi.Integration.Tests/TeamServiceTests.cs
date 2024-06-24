using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using PokemonWebApi.Repositories;
using PokemonWebApi.Services;

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
        public async Task CreateTeamAsync_TooManyPokemons_ThrowsArgumentException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateTeam_TooMany_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var teamService = new TeamService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                var teamDto = new TeamDto
                {
                    TeamName = "Team B",
                    Pokemons = new List<PokemonDto>
                    {
                        new PokemonDto { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<MoveDto> { new MoveDto { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 } } },
                        new PokemonDto { PokemonName = "Charmander", PokemonType = "Fire", Moves = new List<MoveDto> { new MoveDto { MoveName = "Flamethrower", MoveType = "Fire", MovePower = 90 } } },
                        new PokemonDto { PokemonName = "Squirtle", PokemonType = "Water", Moves = new List<MoveDto> { new MoveDto { MoveName = "Water Gun", MoveType = "Water", MovePower = 40 } } },
                        new PokemonDto { PokemonName = "Bulbasaur", PokemonType = "Grass", Moves = new List<MoveDto> { new MoveDto { MoveName = "Vine Whip", MoveType = "Grass", MovePower = 45 } } },
                        new PokemonDto { PokemonName = "Jigglypuff", PokemonType = "Fairy", Moves = new List<MoveDto> { new MoveDto { MoveName = "Sing", MoveType = "Normal", MovePower = 0 } } },
                        new PokemonDto { PokemonName = "Gengar", PokemonType = "Ghost", Moves = new List<MoveDto> { new MoveDto { MoveName = "Shadow Ball", MoveType = "Ghost", MovePower = 80 } } },
                        new PokemonDto { PokemonName = "Eevee", PokemonType = "Normal", Moves = new List<MoveDto> { new MoveDto { MoveName = "Tackle", MoveType = "Normal", MovePower = 40 } } } // 7th Pokémon
                    }
                };

                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(async () => await teamService.CreateTeamAsync(teamDto));
            }
        }
    }
}
