using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_s_Land.DTOs;
using Pet_s_Land.Datas;
using Pet_s_Land.Servies;
using System;
using System.Linq;
using System.Threading.Tasks;
using Pet_s_Land.Repositories;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepoRegister _userRepoRegister;
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService, IUserRepoRegister userRepoRegister)
    {
        _context = context;
        _jwtService = jwtService;
        _userRepoRegister = userRepoRegister;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // Retrieve user from the database
        var user = await _context.Users
            .Where(u => u.UserName == request.Username)
            .Select(u => new { u.UserName, u.Role, u.Id, u.Password }) // Fetch the hashed password
            .FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Generate JWT Token
        var token = _jwtService.GenerateToken(user.UserName, user.Role, user.Id);

        // ✅ Fetch token expiry correctly
        var expiration = DateTime.UtcNow.AddMinutes(_jwtService.GetTokenExpiryMinutes());

        // Return token and expiration time
        return Ok(new JwtResponseDto
        {
            Token = token,
            Expiration = expiration
        });
    }
    [HttpPost("SignUp")]
    public async Task<ActionResult> RegisterUser(UserDto newUser)
    {
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
