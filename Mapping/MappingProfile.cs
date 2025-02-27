using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.AdressModels;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Models.UserModels;
using Pet_s_Land.Models.WhishlistModel;
using static Pet_s_Land.DTOs.UserViewDto;

namespace Pet_s_Land.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() { 
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<ProductDto, Product>()
               .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

            CreateMap<WishListDto, WishList>();
            CreateMap<Address, AddressResDto>();
            CreateMap<AddProductDto, Product>();
            CreateMap<Product, AddProductRes>();
            CreateMap<Product, ProductDto>()
                .ForMember(src => src.Category, opt => opt.MapFrom(src => src.Category.CategoryName));
            CreateMap<User, UserViewDTO>();






        }
    }
}
