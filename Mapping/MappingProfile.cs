using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Models.UserModels;
using Pet_s_Land.Models.WhishlistModel;

namespace Pet_s_Land.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() { 
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<ProductDto, Product>()
               .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

            CreateMap<WishListDto,WishList>().ReverseMap();


        }
    }
}
