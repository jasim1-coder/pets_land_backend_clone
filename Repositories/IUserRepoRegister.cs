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

    }
    public class UserRepoRegister : IUserRepoRegister
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public UserRepoRegister(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseDto<object>> RegisterUser(UserDto regdata)
        {
            var existing = await _appDbContext.Users.FirstOrDefaultAsync(user => user.Email == regdata.Email);
            try
            {
                if (existing != null)
                {
                    return new ResponseDto<object>(null, "User already exists", 400, "Email is already taken.");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(regdata.Password);
                var user = _mapper.Map<User>(regdata);
                user.Password = hashedPassword;
                await _appDbContext.Users.AddAsync(user);
                await _appDbContext.SaveChangesAsync();

                var userDto = _mapper.Map<User>(user);
                return new ResponseDto<object>(userDto, "User registered successfully", 201);

            }
            catch (Exception ex)
            {
                return new ResponseDto<object>(null, "An error occurred", 500, ex.Message);

            }

        }

    }
}