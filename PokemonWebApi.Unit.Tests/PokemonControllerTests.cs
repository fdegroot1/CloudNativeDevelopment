using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokemonWebApi.Controllers;
using PokemonWebApi.Data;
using PokemonWebApi.Models;

namespace PokemonWebApi.Unit.Tests
{
    public class PokemonControllerTests
    {
        
        [Fact]
        public async Task CreatePokemon_Test()
        {
            Pokemon pokemon = new Pokemon { PokemonId = 4, PokemonName = "Mankey" };
            //Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateTestDatabase")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                var controller = new PokemonController(context);

                //Act
                var result = await controller.Create(pokemon);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var createdPokemon = Assert.IsType<Pokemon>(okResult.Value);
                Assert.Equal("Mankey", createdPokemon.PokemonName);

            }
        }
        [Fact]
        public async Task UpdatePokemon_Test()
        {
            // Arrange
            var pokemon = new Pokemon { PokemonId = 3, PokemonName = "Pidgey" };

            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateTestDatabase")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                context.Pokemons.Add(pokemon);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var controller = new PokemonController(context);

                // Modify the Pokemon object (e.g., change its name)
                pokemon.PokemonName = "Pidgeot";

                // Act
                var result = await controller.Update(pokemon);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var updatedPokemon = Assert.IsType<Pokemon>(okResult.Value);
                Assert.Equal("Pidgeot", updatedPokemon.PokemonName);

                // Verify that the Pokemon was updated in the database
                using (var dbContext = new PokemonDbContext(options))
                {
                    var storedPokemon = await dbContext.Pokemons.FindAsync(3);
                    Assert.NotNull(storedPokemon);
                    Assert.Equal("Pidgeot", storedPokemon.PokemonName);
                }
            }
        }
        [Fact]
        public async Task GetPokemonById_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "GetByIdTestDatabase")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                context.Pokemons.Add(new Models.Pokemon { PokemonId = 1, PokemonName = "Pikachu" });
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var controller = new PokemonController(context);

                //Act
                var result = await controller.GetById(1);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<OkObjectResult>(result.Result); // Ensure it's an OkObjectResult

                // Inspect the value of the result
                var okResult = result.Result as OkObjectResult;
                var pokemon = Assert.IsType<Pokemon>(okResult.Value);
                Assert.Equal("Pikachu", pokemon.PokemonName);

            }
        }
        [Fact]
        public async Task DeletePokemonById_Test()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteTestDatabase")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                // Add a Pokemon to be deleted
                var pokemon = new Pokemon { PokemonId = 2, PokemonName = "Charmander" };
                context.Pokemons.Add(pokemon);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var controller = new PokemonController(context);

                // Act
                var result = await controller.Delete(2);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<OkObjectResult>(result);

                // Verify that the Pokemon was deleted from the database
                using (var dbContext = new PokemonDbContext(options))
                {
                    var deletePokemon = await dbContext.Pokemons.FindAsync(2);
                    Assert.Null(deletePokemon);
                }

                // Inspect the value of the result
                var okResult = result as OkObjectResult;
                var deletedPokemon = Assert.IsType<Pokemon>(okResult.Value);
                Assert.Equal("Charmander", deletedPokemon.PokemonName);
            }
        }
        [Fact]
        public void GetPokemons_Test()
        {
            // Arrange
            var pokemons = new List<Pokemon>
            {
                new Pokemon { PokemonId = 1, PokemonName = "Pikachu" },
                new Pokemon { PokemonId = 2, PokemonName = "Charmander" }
            };

            var options = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseInMemoryDatabase(databaseName: "GetPokemonsTestDatabase")
                .Options;

            using (var context = new PokemonDbContext(options))
            {
                context.Pokemons.AddRange(pokemons);
                context.SaveChanges();
            }

            using (var context = new PokemonDbContext(options))
            {
                var controller = new PokemonController(context);

                // Act
                var result = controller.GetPokemons();

                // Assert
                var returnedPokemons = Assert.IsAssignableFrom<IEnumerable<Pokemon>>(result.Value);
                Assert.Equal(pokemons.Count, returnedPokemons.Count());
                // Additional assertions can be made to verify the contents of the returned collection
            }
        }
    }
}