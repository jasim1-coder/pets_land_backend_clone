using System.Collections.Generic;
using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IWishListServices
    {
        Task<ResponseDto<object>> AddorRemove(int userId, int productId);
        Task<ResponseDto<List<WishListResDto>>> GetWishList(int userId);
    }

    public class WishListServices : IWishListServices
    {
        private readonly IWishListRep _wishListRep;
        private readonly IMapper _mapper;

        public WishListServices(IWishListRep wishListRep, IMapper mapper)
        {
            wishListRep = _wishListRep;
            mapper = _mapper;
        }


            public async Task<ResponseDto<object>> AddorRemove(int userId, int productId)
             {
            return await _wishListRep.AddorRemove(userId, productId);
             }
            
        public async Task<ResponseDto<List<WishListResDto>>> GetWishList(int userId)

        {
            return await _wishListRep.GetWishList(userId);

        }





    }
}
