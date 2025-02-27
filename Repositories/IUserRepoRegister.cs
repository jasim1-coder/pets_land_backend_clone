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
        Task<ResponseDto<object>> RegisterUser(UserDto userDto);
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
                bool existing = await _appDbContext.Users.AnyAsync(user => user.Email == regdata.Email || user.UserName == regdata.UserName);

                if (existing)
                {
                    return new ResponseDto<object>(null, "User already exists", 400, "Email or UserName is already taken.");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(regdata.Password);
                var user = _mapper.Map<User>(regdata);
                user.Password = hashedPassword;
                await _appDbContext.Users.AddAsync(user);
                await _appDbContext.SaveChangesAsync();

                //var userDto = _mapper.Map<User>(user);
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
                    Expiration = expiration
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