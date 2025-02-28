using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Servies;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserServices _userServices;

    public AuthController(IUserServices userServices)
    {

        _userServices = userServices;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await _userServices.LoginUser(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> RegisterUser(UserDto regdata)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResponseDto<object>(null, "Invalid input data",400));
        }
        var result = await _userServices.RegisterUser(regdata);

        return StatusCode(result.StatusCode,result);
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var response = await _userServices.RefreshToken(request.RefreshToken);

        if (response.StatusCode == 200)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }

}
