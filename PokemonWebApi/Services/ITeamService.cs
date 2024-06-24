// /Services/ITeamService.cs
using PokemonWebApi.Models;
using System.Threading.Tasks;

namespace PokemonWebApi.Services
{
    public interface ITeamService
    {
        Task<TeamDto> CreateTeamAsync(TeamDto teamDto);
        Task<TeamDto> GetTeamByIdAsync(int id);
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto> GetTeamByNameAsync(string name);
        Task<bool> DeleteTeamByNameAsync(string name);
    }
}
