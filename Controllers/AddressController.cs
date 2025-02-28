using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Servies;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]

    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("AddAddress")]
        public async Task<ActionResult> AddAddress(AddressCreateDto newAddress)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            int UserId = int.Parse(userIdClaim.Value);

            var result = await _addressService.AddAddress(UserId, newAddress);
            return StatusCode(result.StatusCode,result);
        }

        [HttpDelete("DeleteAddress")]
        public async Task<ActionResult> RemoveAddress(int addressId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            int UserId = int.Parse(userIdClaim.Value);
            var result = await _addressService.RemoveAddress(UserId, addressId);

            return StatusCode(result.StatusCode,result);
        }


        [HttpGet("GetAllAddress")]

        public async Task<ActionResult> GetAddresses()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");

            int UserId = int.Parse(userIdClaim.Value);
            var result = await _addressService.GetAddresses(UserId);
            return StatusCode(result.StatusCode,result);

        }


    }
}
