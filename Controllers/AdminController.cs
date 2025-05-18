using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Enums;
using Pet_s_Land.Repositories;
using Pet_s_Land.Servies;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 

    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminServices;

        public AdminController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

        [HttpGet("Get-All-Users")]
        public async Task<ActionResult> GetAllUser()
        {
            var result = await _adminServices.GetAllUser();
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("Get-UserBy-Id")]

        public async Task<ActionResult> GetUser(int id) 
        {
            var result = await _adminServices.GetUserById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Block-Or-Unblock-User")]

        public async Task<ActionResult>BlockOrUnblock(int Id)
        {
            var result = await _adminServices.BlockOrUnblockUser(Id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("total-products-purchased")]
        public async Task<IActionResult> GetTotalProductsPurchased()
        {
            var response = await _adminServices.TotalProductPurchased();
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var response = await _adminServices.GetTotalRevenue();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllOrdersWithItems()
        {
            var response = await _adminServices.GetAllOrdersWithItems();
            return StatusCode(response.StatusCode, response);
        }



        //[HttpGet("{userId}/orders")]
        //public async Task<IActionResult> GetUserOrders(int userId)
        //{
        //    try
        //    {
        //        var response = await _orderService.GetUserOrders(userId);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error in GetUserOrders API: {ex.Message}");
        //        return StatusCode(500, new ResponseDto<string>(null, "Internal server error while fetching user orders.", 500));
        //    }
        //}


        [HttpGet("User-Order")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var respnse = await _adminServices.GetUserOrders(userId);
            return StatusCode(Response.StatusCode, respnse);
        }
        [HttpGet("filter-orders")]
        public async Task<IActionResult> FilterOrderDetails([FromQuery] int? userId, [FromQuery] OrderStatusEnum? status, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {

            var response = await _adminServices.FilterOrderDetails(userId, status, startDate, endDate);
            return StatusCode(response.StatusCode, response);
        }

        //[HttpGet("filter-orders")]
        //public async Task<IActionResult> FilterOrderDetails(int? userId, string? status, DateTime? startDate, DateTime? endDate)
        //{
        //    // Convert the status string to OrderStatusEnum
        //    OrderStatusEnum? orderStatus = null;
        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        if (Enum.TryParse(typeof(OrderStatusEnum), status, true, out var parsedStatus))
        //        {
        //            orderStatus = (OrderStatusEnum)parsedStatus;
        //        }
        //        else
        //        {
        //            return BadRequest(new ResponseDto<List<OrderDto>>(null, "Invalid order status.", 400));
        //        }
        //    }

        //    var response = await _adminServices.FilterOrderDetails(userId, orderStatus, startDate, endDate);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpPut("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatusEnum newStatus)
        {

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            int adminId = int.Parse(userIdClaim.Value);
            var result = await _adminServices.UpdateOrderStatus(orderId,newStatus, adminId);
            return StatusCode(result.StatusCode, result);
        }



        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var response = await _adminServices.DeleteProduct(productId);
            return  StatusCode(response.StatusCode, response);

        }


        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] AddProductDto productData)
        {
            var result = await _adminServices.UpdateProductAsync(productId, productData);
            return StatusCode(result.StatusCode, result);
        }


    }
}
