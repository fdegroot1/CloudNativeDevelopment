using AutoMapper;
using PokemonWebApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PokemonWebApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamDto>().ReverseMap();
            CreateMap<Pokemon, PokemonDto>().ReverseMap();
            CreateMap<Move, MoveDto>().ReverseMap();
        }
    }
}
