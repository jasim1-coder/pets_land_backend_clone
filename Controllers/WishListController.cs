using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListRep _wishListRep;

        public WishListController(IWishListRep wishListRep)
        {
            _wishListRep = wishListRep;
        }

        [HttpPost("AddOrRemoveFrom WishList")]
        public async Task<ResponseDto<object>> AddOrRemoveFromWishlist(int ProductId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return new ResponseDto<object>(null, "Unauthorized: User ID not found.", 401, "Invalid Token");
            }

            int userId = int.Parse(userIdClaim.Value);

            return await _wishListRep.AddorRemove(userId, ProductId);
        }


        [HttpGet("GetWishList")]
        public async Task<ResponseDto<List<WishListResDto>>> GetWishlist()
        {
            // Extract UserId from JWT token
            var userIdClaim = HttpContext.User.FindFirst("UserId");

            // Check if UserId exists
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return new ResponseDto<List<WishListResDto>>(null, "Unauthorized: User ID not found.", 401);
            }

            // Convert UserId to int
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return new ResponseDto<List<WishListResDto>>(null, "Invalid User ID format.", 400);
            }

            // Call repository method
            return await _wishListRep.GetWishList(userId);
        }

    }
}
