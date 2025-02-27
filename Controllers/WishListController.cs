using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Repositories;
using Pet_s_Land.Servies;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListServices _wishListServices;

        public WishListController(IWishListServices wishListServices)
        {
            _wishListServices = wishListServices;
        }

        [HttpPost("AddOrRemoveFrom WishList")]
        public async Task<ActionResult> AddOrRemoveFromWishlist(int ProductId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return  BadRequest( new ResponseDto<object>(null, "Unauthorized: User ID not found.", 401, "Invalid Token"));
            }

            int userId = int.Parse(userIdClaim.Value);
            var result = await _wishListServices.AddorRemove(userId, ProductId);


            return StatusCode(result.StatusCode,result);
        }


        [HttpGet("GetWishList")]
        public async Task<ActionResult> GetWishlist()
        {
            var userIdClaim = HttpContext.User.FindFirst("UserId");

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return BadRequest( new ResponseDto<List<WishListResDto>>(null, "Unauthorized: User ID not found.", 401));
            }
            int userId = int.Parse(userIdClaim.Value);
            var result = await _wishListServices.GetWishList(userId);

            return StatusCode(result.StatusCode,result);
        }

    }
}
