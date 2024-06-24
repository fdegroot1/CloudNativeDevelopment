using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using PokemonWebApi.Repositories;
using PokemonWebApi.Services;
using Xunit;

namespace PokemonWebApi.Integration.Tests.Services
{
    public class MoveServiceTests
    {
        private readonly Mock<IPokeApiService> _pokeApiServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly HttpClient _httpClient;

        public MoveServiceTests()
        {
            _pokeApiServiceMock = new Mock<IPokeApiService>();
            _mapperMock = new Mock<IMapper>();
            _httpClient = new HttpClient();
        }

        [Fact]
        public async Task AddMoveToPokemonAsync_ValidMove_AddsMove()
        {
            // Arrange
            var _dbContextOptions = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "AddMove_Valid_db")
                .Options;
            _pokeApiServiceMock.Setup(s => s.CanLearnMoveAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            var moveDto = new MoveDto { MoveName = "Quick Attack", MoveType = "Normal", MovePower = 40 };
            var moveEntity = new Move { MoveName = "Quick Attack", MoveType = "Normal", MovePower = 40 };
            _mapperMock.Setup(m => m.Map<Move>(moveDto)).Returns(moveEntity);

            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move>() };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var moveService = new MoveService(context, new PokemonRepository(_httpClient), _mapperMock.Object, _pokeApiServiceMock.Object);

                // Act
                var result = await moveService.AddMoveToPokemonAsync(1, 1, moveDto);

                // Assert
                Assert.True(result);
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Contains(pokemon.Moves, m => m.MoveName == "Quick Attack");
            }
        }

        [Fact]
        public async Task AddMoveToPokemonAsync_InvalidMove_ReturnsFalse()
        {
            // Arrange
            var _dbContextOptions = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "AddMove_Invalid_db")
                .Options;
            _pokeApiServiceMock.Setup(s => s.CanLearnMoveAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            var moveDto = new MoveDto { MoveName = "Quick Attack", MoveType = "Normal", MovePower = 40 };

            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move>() };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var moveService = new MoveService(context, new PokemonRepository(_httpClient), _mapperMock.Object, _pokeApiServiceMock.Object);

                // Act
                var result = await moveService.AddMoveToPokemonAsync(1, 1, moveDto);

                // Assert
                Assert.False(result);
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.DoesNotContain(pokemon.Moves, m => m.MoveName == "Quick Attack");
            }
        }

        [Fact]
        public async Task DeletePokemonMoveAsync_ValidMove_DeletesMove()
        {
            // Arrange
            var _dbContextOptions = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteMove_Valid_db")
                .Options;
            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move> { new Move { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 } } };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var moveService = new MoveService(context, new PokemonRepository(_httpClient), _mapperMock.Object, _pokeApiServiceMock.Object);

                // Act
                var result = await moveService.DeletePokemonMoveAsync(1, 1, "Thunderbolt");

                // Assert
                Assert.True(result);
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Empty(pokemon.Moves);
            }
        }

        [Fact]
        public async Task DeletePokemonMoveAsync_InvalidMove_ReturnsFalse()
        {
            // Arrange
            var _dbContextOptions = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteMove_Invalid_db")
                .Options;
            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move> { new Move { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 } } };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(_dbContextOptions))
            {
                var moveService = new MoveService(context, new PokemonRepository(_httpClient), _mapperMock.Object, _pokeApiServiceMock.Object);

                // Act
                var result = await moveService.DeletePokemonMoveAsync(1, 1, "Surf");

                // Assert
                Assert.False(result);
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Single(pokemon.Moves);
                Assert.Contains(pokemon.Moves, m => m.MoveName == "Thunderbolt");
            }
        }
    }
}
