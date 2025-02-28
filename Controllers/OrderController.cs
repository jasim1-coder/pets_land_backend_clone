using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Services;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public OrderController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("CreateOrder")]
        public async Task<ResponseDto<string>> RazorOrderCreate(long price)
        {
            return await _paymentService.RazorOrderCreate(price);
        }

        [HttpPost("MakeRazorPayment")]

        public async Task<ResponseDto<bool>> RazorPayment(PaymentDto payment)
        {

            return await _paymentService.RazorPayment(payment);
        }

        [HttpPost("ConfirmOrder")]

        public async Task<ResponseDto<bool>> CreateOrder(CreateOrderDto createOrderDTO)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            int UserId = int.Parse(userIdClaim.Value);
            return await _paymentService.CreateOrder(UserId, createOrderDTO);
        }


        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder(CreateOrderDto createOrderDTO)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            int UserId = int.Parse(userIdClaim.Value);

            if (createOrderDTO == null)
            {
                return BadRequest(new ResponseDto<bool>(false, "Invalid order details", 400));
            }

            var response = await _paymentService.PlaceOrder(UserId, createOrderDTO);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("Get-order-details")]
        public async Task<ActionResult> GetUserOrders()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            int UserId = int.Parse(userIdClaim.Value);
            var result = await _paymentService.GetUserOrders(UserId);
            return StatusCode(result.StatusCode, result);

        }


    }
}
    