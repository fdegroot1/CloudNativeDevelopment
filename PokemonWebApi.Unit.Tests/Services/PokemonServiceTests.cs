using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokemonWebApi.Data;
using PokemonWebApi.Models;
using PokemonWebApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PokemonWebApi.Unit.Tests.Services
{
    public class PokemonServiceTests
    {
        [Fact]
        public async Task AddPokemonToTeamAsync_ValidPokemon_AddsPokemon()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "AddPokemon_Valid_db")
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
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var pokemonDto = new PokemonDto { PokemonName = "Meowth", PokemonType = "Normal" };
                var pokemon = new Pokemon { PokemonName = "Meowth", PokemonType = "Normal", Moves = new List<Move> { new Move { MoveName = "Scratch", MoveType = "Normal", MovePower = 40 } } };

                mapperMock.Setup(mock => mock.Map<Pokemon>(pokemonDto)).Returns(pokemon);

                var pokemonService = new PokemonService(context, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await pokemonService.AddPokemonToTeamAsync(1, pokemonDto);

                // Assert
                Assert.True(result);

                // Verify Pokemon was added to the team
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                Assert.Equal(2,team.Pokemons.Count); // Ensure only one Pokemon is added
            }
        }

        [Fact]
        public async Task AddPokemonToTeamAsync_TooManyPokemon_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "AddPokemon_TooMany_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                team.Pokemons.AddRange(new List<Pokemon>
                {
                    new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric" },
                    new Pokemon { PokemonName = "Bulbasaur", PokemonType = "Grass" },
                    new Pokemon { PokemonName = "Charmander", PokemonType = "Fire" },
                    new Pokemon { PokemonName = "Squirtle", PokemonType = "Water" },
                    new Pokemon { PokemonName = "Jigglypuff", PokemonType = "Fairy" },
                    new Pokemon { PokemonName = "Snorlax", PokemonType = "Normal" }
                });
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var pokemonDto = new PokemonDto { PokemonName = "Mewtwo", PokemonType = "Psychic" };
                var pokemon = new Pokemon { PokemonName = "Mewtwo", PokemonType = "Psychic" };

                mapperMock.Setup(mock => mock.Map<Pokemon>(pokemonDto)).Returns(pokemon);

                var pokemonService = new PokemonService(context, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await pokemonService.AddPokemonToTeamAsync(1, pokemonDto);

                // Assert
                Assert.False(result);

                // Verify no additional Pokemon was added to the team
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                Assert.Equal(6, team.Pokemons.Count); // Ensure team size is unchanged
            }
        }

        [Fact]
        public async Task RemovePokemonFromTeamAsync_ValidPokemon_RemovesPokemon()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "RemovePokemon_Valid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                team.Pokemons.AddRange(new List<Pokemon>
                {
                    new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric" },
                    new Pokemon { PokemonName = "Bulbasaur", PokemonType = "Grass" },
                    new Pokemon { PokemonName = "Charmander", PokemonType = "Fire" },
                    new Pokemon { PokemonName = "Squirtle", PokemonType = "Water" },
                    new Pokemon { PokemonName = "Jigglypuff", PokemonType = "Fairy" },
                    new Pokemon { PokemonName = "Snorlax", PokemonType = "Normal" }
                });
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var pokemonService = new PokemonService(context, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await pokemonService.RemovePokemonFromTeamAsync(1, 1);

                // Assert
                Assert.True(result);

                // Verify Pokemon was removed from the team
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                Assert.Equal(5, team.Pokemons.Count); // Ensure no Pokemon left in the team
            }
        }

        [Fact]
        public async Task RemovePokemonFromTeamAsync_InvalidPokemon_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "RemovePokemon_Invalid_db")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Seed database with test data
                var team = new Team { TeamName = "Team A", Pokemons = new List<Pokemon>() };
                var pokemonToRemove = new Pokemon { PokemonName = "Pikachu", PokemonType = "Electric" };
                team.Pokemons.Add(pokemonToRemove);
                context.Teams.Add(team);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var mapperMock = new Mock<IMapper>();
                var pokeApiServiceMock = new Mock<IPokeApiService>();

                var pokemonService = new PokemonService(context, mapperMock.Object, pokeApiServiceMock.Object);

                // Act
                var result = await pokemonService.RemovePokemonFromTeamAsync(1, 2); // Pokemon with ID 2 does not exist

                // Assert
                Assert.False(result);

                // Verify no change in team
                var team = await context.Teams
                    .Include(t => t.Pokemons)
                    .FirstOrDefaultAsync(t => t.TeamId == 1);

                Assert.NotNull(team);
                Assert.Single(team.Pokemons); // Ensure only one Pokemon left in the team
            }
        }
    }
}

