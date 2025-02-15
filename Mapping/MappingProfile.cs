using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.UserModels;

namespace Pet_s_Land.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() { 
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
    }
    }
}
