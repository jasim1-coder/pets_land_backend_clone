using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Models.UserModels;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly ICartRepo _cartRepo;

        public CartController(ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
        }

        [HttpGet("GetCart Items")]
        public async Task<ResponseDto<CartResDto>> GetCartItems()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return new ResponseDto<CartResDto>(null, "Unauthorized: User ID not found.", 401, "Invalid Token");
            }

            int UserId = int.Parse(userIdClaim.Value);

            return await  _cartRepo.GetCartItems(UserId);
        }

        [HttpPost("AddToCart")]
        public async Task<ResponseDto<object>> AddToCart(int productId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return new ResponseDto<object>(null, "Unauthorized: User ID not found.", 401, "Invalid Token");
            }

            int UserId = int.Parse(userIdClaim.Value);

            return await _cartRepo.AddToCart(UserId, productId);

        }

    }

}
