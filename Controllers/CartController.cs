using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;

using Pet_s_Land.Repositories;
using Pet_s_Land.Servies;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly ICartServices _cartServices;

        public CartController(ICartServices cartServices)
        {
            _cartServices = cartServices;
        }

        [HttpGet("GetCart Items")]
        public async Task<ActionResult> GetCartItems()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return BadRequest(new ResponseDto<bool>(false, "Unauthorized: User ID not found.", 401, "Invalid Token"));
            }

            int UserId = int.Parse(userIdClaim.Value);
            var result = await _cartServices.GetCartItems(UserId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("AddToCart")]
        public async Task<ActionResult> AddToCart(int productId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return BadRequest(new ResponseDto<bool>(false, "Unauthorized: User ID not found.", 401, "Invalid Token"));
            }

            int UserId = int.Parse(userIdClaim.Value);
            var result = await _cartServices.AddToCart(UserId, productId);

            return StatusCode(result.StatusCode,result);

        }

        [HttpDelete("DeleteProductFromCart")]
        public async Task<ActionResult> RemoveFromCart(int productId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return BadRequest(new ResponseDto<bool>(false, "Unauthorized: User ID not found.", 401, "Invalid Token"));
            }

            int UserId = int.Parse(userIdClaim.Value);
            var result = await _cartServices.RemoveFromCart(UserId,productId);

            return StatusCode(result.StatusCode,result);
        }

        [HttpPost("IncreaseQuantity")]
        public async Task<ActionResult> IncreaseQty(int productId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null)
            {
                return BadRequest(new ResponseDto<bool>(false, "Unauthorized: User ID not found.", 401, "Invalid Token"));
            }

            int UserId = int.Parse(userIdClaim.Value);
            var result = await _cartServices.IncreaseQty(UserId,productId);
            return StatusCode(result.StatusCode,result);
        }

        [HttpPost("DecreaseQuantity")]
        public async Task<ActionResult> DecreaseQty(int productId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return BadRequest(new ResponseDto<bool>(false, "Unauthorized: User ID not found.", 401, "Invalid Token"));
            }
            int UserId = int.Parse(userIdClaim.Value);
            var result = await _cartServices.DecreaseQty(UserId,productId);
            return StatusCode(result.StatusCode,result);
        }

        [HttpDelete("RomeveAllItemsFromCart")]
        public async Task<ActionResult> RemoveAllItems()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return BadRequest(new ResponseDto<bool>(false, "Unauthorized: User ID not found.", 401, "Invalid Token"));
            }
            int UserId = int.Parse(userIdClaim.Value);
            var result = await _cartServices.RemoveAllItems(UserId);
            return StatusCode(result.StatusCode,result);

        }

    }

}
