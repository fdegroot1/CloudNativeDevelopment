using Microsoft.EntityFrameworkCore;
using PokemonWebApi.Models;

namespace PokemonWebApi.Data
{
    public interface IPokemonDbContext
    {
        DbSet<Team> Teams { get; set; }
        DbSet<Pokemon> Pokemons { get; set; }
        DbSet<Move> Moves { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
