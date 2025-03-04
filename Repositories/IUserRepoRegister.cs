using AutoMapper;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using BCrypt.Net;
using Pet_s_Land.Models.UserModels;
using Microsoft.EntityFrameworkCore;


namespace Pet_s_Land.Repositories
{
    public interface IUserRepoRegister
    {
        Task<ResponseDto<object>> RegisterUser(UserDto regdata);
        Task<ResponseDto<JwtResponseDto>> LoginUser(LoginRequestDto loginData);
        Task<ResponseDto<JwtResponseDto>> RefreshToken(string refreshToken);


    }
    public class UserRepoRegister : IUserRepoRegister
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;


        public UserRepoRegister(AppDbContext appDbContext, IMapper mapper, JwtService jwtService)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _jwtService = jwtService;

        }

        public async Task<ResponseDto<object>> RegisterUser(UserDto regdata)
        {
            try
            {
                // Ensure case-insensitive check for existing users
                var user = await _appDbContext.Users
                    .Where(u => u.Email.ToLower() == regdata.Email.ToLower() || u.UserName.ToLower() == regdata.UserName.ToLower())
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    Console.WriteLine($"Existing User: Email = {user.Email}, Name = {user.Name}");
                    return new ResponseDto<object>(null, "User already exists", 400, "Email or UserName is already taken.");
                }

                // Validate phone number before proceeding
                if (string.IsNullOrWhiteSpace(regdata.PhoneNo))
                {
                    return new ResponseDto<object>(null, "Phone number is required", 400, "Phone number cannot be empty.");
                }

                // Hash password before saving
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(regdata.Password);

                // Create a new user object
                var users = new User
                {
                    Name = regdata.Name,
                    Email = regdata.Email,
                    UserName = regdata.UserName,
                    Password = hashedPassword,  // Assign hashed password directly
                    PhoneNumber = regdata.PhoneNo,  // Assign after validation
                    Role = "User"
                };

                // Add user to the database
                await _appDbContext.Users.AddAsync(users);
                await _appDbContext.SaveChangesAsync();

                // Prepare response DTO
                var result = new UserResDto
                {
                    Name = regdata.Name,
                    UserName = regdata.UserName,
                    Email = regdata.Email,
                    PhoneNo = regdata.PhoneNo,
                    Role = "User"
                };

                return new ResponseDto<object>(result, "User registered successfully", 201);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                return new ResponseDto<object>(null, "An error occurred", 500, ex.Message);
            }
        }



        public async Task<ResponseDto<JwtResponseDto>> LoginUser(LoginRequestDto loginData)
        {
            try
            {
                var user = await _appDbContext.Users
                    .Where(u => u.UserName == loginData.Username)
                    .Select(u => new { u.UserName, u.Role, u.Id, u.Password, u.IsBlocked })  
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return new ResponseDto<JwtResponseDto>(null, "Invalid credentials", 400);
                }

                if (user.IsBlocked)
                {
                    return new ResponseDto<JwtResponseDto>(null, "User is blocked by admin", 400);
                }

                if (!BCrypt.Net.BCrypt.Verify(loginData.Password, user.Password))
                {
                    return new ResponseDto<JwtResponseDto>(null, "Invalid credentials", 400);
                }
                var Name = user.UserName;
                var Id = user.Id;
                var token = _jwtService.GenerateToken(user.UserName, user.Role, user.Id);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var expiration = DateTime.UtcNow.AddMinutes(_jwtService.GetTokenExpiryMinutes());

                // Store refresh token in the database
                var dbUser = await _appDbContext.Users.FindAsync(user.Id);
                if (dbUser != null)
                {
                    dbUser.RefreshToken = refreshToken;
                    dbUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days
                    await _appDbContext.SaveChangesAsync();
                }


                var response = new JwtResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = expiration,
                    Name = Name,
                    UserId = Id
                };

                return new ResponseDto<JwtResponseDto>(response, "Login successful", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<JwtResponseDto>(null, "An error occurred", 500, ex.Message);
            }
        }
        public async Task<ResponseDto<JwtResponseDto>> RefreshToken(string refreshToken)
        {
            try
            {
                var user = await _appDbContext.Users
                    .Where(u => u.RefreshToken == refreshToken)
                    .FirstOrDefaultAsync();

                if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    return new ResponseDto<JwtResponseDto>(null, "Invalid or expired refresh token", 400);
                }

                // Generate new JWT and Refresh Token
                var newToken = _jwtService.GenerateToken(user.UserName, user.Role, user.Id);
                var newRefreshToken = _jwtService.GenerateRefreshToken();
                var expiration = DateTime.UtcNow.AddMinutes(_jwtService.GetTokenExpiryMinutes());

                // Update the database with the new refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _appDbContext.SaveChangesAsync();

                var response = new JwtResponseDto
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    Expiration = expiration
                };

                return new ResponseDto<JwtResponseDto>(response, "Token refreshed successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<JwtResponseDto>(null, "An error occurred", 500, ex.Message);
            }
        }

    }
}