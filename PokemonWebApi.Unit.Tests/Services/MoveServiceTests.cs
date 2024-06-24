using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using PokemonWebApi.Repositories;
using PokemonWebApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PokemonWebApi.Unit.Tests.Services
{
    public class MoveServiceTests
    {
        [Fact]
        public async Task DeletePokemonMoveAsync_ValidMove_DeletesMove()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "DeletePokemonMove_Valid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move> { new Move { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 } } };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var moveService = new MoveService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await moveService.DeletePokemonMoveAsync(1, 1, "Thunderbolt");

                // Assert
                Assert.True(result);

                // Verify deletion
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Empty(pokemon.Moves); // Ensure move is deleted
            }
        }

        [Fact]
        public async Task DeletePokemonMoveAsync_InvalidMove_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "DeletePokemonMove_Invalid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move> { new Move { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 } } };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var moveService = new MoveService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await moveService.DeletePokemonMoveAsync(1, 1, "Surf"); // "Surf" move does not exist

                // Assert
                Assert.False(result);

                // Verify no change in database
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Single(pokemon.Moves); // Ensure move count is unchanged
            }
        }
        [Fact]
        public async Task AddMoveToPokemonAsync_ValidMove_AddsMove()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "AddMove_Valid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move> { new Move { MoveName = "Quick-Attack", MoveType = "Normal", MovePower = 40 } } };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                // Mock behavior of PokeApiService.CanLearnMoveAsync to return true
                pokeApiServiceMock.Setup(mock => mock.CanLearnMoveAsync("pikachu", "thunderbolt")).ReturnsAsync(true);

                var moveDto = new MoveDto { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 };
                var move = new Move { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 };

                mapperMock.Setup(mock => mock.Map<Move>(moveDto)).Returns(move);

                var moveService = new MoveService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await moveService.AddMoveToPokemonAsync(1, 1, moveDto);

                // Assert
                Assert.True(result);

                // Verify move was added to the Pokemon
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Equal(2,pokemon.Moves.Count); // Ensure move count is 2 after addition
            }
        }

        [Fact]
        public async Task AddMoveToPokemonAsync_TooManyMoves_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "AddMove_TooMany_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon
                {
                    PokemonName = "Pikachu",
                    PokemonType = "Electric",
                    Moves = new List<Move>
                    {
                        new Move { MoveName = "Thunderbolt", MoveType = "Electric", MovePower = 90 },
                        new Move { MoveName = "Quick-Attack", MoveType = "Normal", MovePower = 40 },
                        new Move { MoveName = "Iron-Tail", MoveType = "Steel", MovePower = 100 },
                        new Move { MoveName = "Thundershock", MoveType = "Electric", MovePower = 40 }
                    }
                };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var moveDto = new MoveDto { MoveName = "Thunder-Wave", MoveType = "Electric", MovePower = 0 };
                var move = new Move { MoveName = "Thunder-Wave", MoveType = "Electric", MovePower = 0 };

                mapperMock.Setup(mock => mock.Map<Move>(moveDto)).Returns(move);

                var moveService = new MoveService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await moveService.AddMoveToPokemonAsync(1, 1, moveDto);

                // Assert
                Assert.False(result);

                // Verify move was not added due to maximum moves limit
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Equal(4, pokemon.Moves.Count); // Ensure move count is unchanged
            }
        }

        [Fact]
        public async Task AddMoveToPokemonAsync_CannotLearnMove_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "AddMove_CannotLearn_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemon = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric", Moves = new List<Move> { new Move { MoveName = "Quick-Attack", MoveType = "Normal", MovePower = 40 } } };
                team.Pokemons.Add(pokemon);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var pokemonRepositoryMock = new Mock<IPokemonRepository>();
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                // Mock behavior of PokeApiService.CanLearnMoveAsync to return false
                pokeApiServiceMock.Setup(mock => mock.CanLearnMoveAsync("pikachu", "surf")).ReturnsAsync(false);

                var moveDto = new MoveDto { MoveName = "Surf", MoveType = "Water", MovePower = 90 };
                var move = new Move { MoveName = "Surf", MoveType = "Water", MovePower = 90 };

                mapperMock.Setup(mock => mock.Map<Move>(moveDto)).Returns(move);

                var moveService = new MoveService(context, pokemonRepositoryMock.Object, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await moveService.AddMoveToPokemonAsync(1, 1, moveDto);

                // Assert
                Assert.False(result);

                // Verify move was not added due to inability to learn move
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .ThenInclude(p => p.Moves)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                var pokemon = team.Pokemons.FirstOrDefault(p => p.PokemonId == 1);
                Assert.NotNull(pokemon);
                Assert.Single(pokemon.Moves); // Ensure no moves were added
            }
        }
    }
}
