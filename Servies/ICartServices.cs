using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.CartModels;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface ICartServices
    {
        Task<ResponseDto<CartResDto>> GetCartItems(int userId);
        Task<ResponseDto<object>> AddToCart(int userId, int productId);
        //Task<ResponseDto<object>> RemoveFromCart(int userId, int productId);
        //Task<ResponseDto<object>> IncreaseQty(int userId, int productId);
        //Task<ResponseDto<object>> DecreaseQty(int userId, int productId);
        //Task<ResponseDto<object>> RemoveAllItems(int userId);

    }

    public class CartServices: ICartServices
    {
        private readonly ICartRepo _cartRepo;
        private readonly IMapper _mapper;

        public CartServices(ICartRepo cartRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _mapper = mapper;
        }

            public async Task<ResponseDto<object>> AddToCart(int userId, int productId)
        {
            return await _cartRepo.AddToCart(userId, productId);
        }
        public async Task<ResponseDto<CartResDto>> GetCartItems(int userId)
        {
            return await _cartRepo.GetCartItems(userId);
        }
        //public async Task<ResponseDto<object>> RemoveFromCart(int userId, int productId)
        //{
        //    return await _cartRepo.RemoveFromCart(userId,productId);
        //}

        //public async Task<ResponseDto<object>> IncreaseQty(int userId, int productId)
        //{
        //    return await _cartRepo.IncreaseQty(userId, productId);
        //}

        //public async Task<ResponseDto<object>> DecreaseQty(int userId, int productId)
        //{
        //    return await _cartRepo.DecreaseQty(userId, productId);
        //}

        //public async Task<ResponseDto<object>> RemoveAllItems(int userId)
        //{
        //    return await _cartRepo.RemoveAllItems(userId);
        //}


    }

}


