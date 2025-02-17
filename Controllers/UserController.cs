using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepoRegister _userRepoRegister;

        public UserController(IUserRepoRegister userRepoRegister)
        {
            _userRepoRegister = userRepoRegister;
        }
        [HttpPost("SignUp")]
        public async Task<ActionResult> RegisterUser(UserDto newUser) {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data", StatusCode = 400 });
            }
            var result = await _userRepoRegister.RegisterUser(newUser);
            if (result.stausCode == 500) 
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(RegisterUser), new { email = newUser.Email }, result);
        }
    }
}
