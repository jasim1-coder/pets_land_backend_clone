using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;

using Pet_s_Land.Servies;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]

    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListServices _wishListServices;

        public WishListController(IWishListServices wishListServices)
        {
            _wishListServices = wishListServices;
        }

        [HttpPost("AddOrRemoveFromWishList")]
        public async Task<ActionResult> AddOrRemoveFromWishlist(int ProductId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            int userId = int.Parse(userIdClaim.Value);
            var result = await _wishListServices.AddorRemove(userId, ProductId);
            return StatusCode(result.StatusCode,result);
        }


        [HttpGet("GetWishList")]
        public async Task<ActionResult> GetWishlist()
        {
            var userIdClaim = HttpContext.User.FindFirst("UserId");
            int userId = int.Parse(userIdClaim.Value);
            var result = await _wishListServices.GetWishList(userId);
            return StatusCode(result.StatusCode,result);
        }

    }
}
