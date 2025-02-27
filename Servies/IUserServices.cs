using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IUserServices
    {
        Task<ResponseDto<object>> RegisterUser(UserDto regdata);
        Task<ResponseDto<JwtResponseDto>> LoginUser(LoginRequestDto loginData);
        Task<ResponseDto<JwtResponseDto>> RefreshToken(string refreshToken);



    }
    public class RegisterUsers : IUserServices
    {
        private readonly IUserRepoRegister _userRepository;

        public RegisterUsers(IUserRepoRegister userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ResponseDto<object>> RegisterUser(UserDto regdata)
        {
            return await _userRepository.RegisterUser(regdata);
        }

        public async Task<ResponseDto<JwtResponseDto>> LoginUser(LoginRequestDto loginData)
        {
            return await _userRepository.LoginUser(loginData);
        }

        public async Task<ResponseDto<JwtResponseDto>> RefreshToken(string refreshToken)
        {
            return await (_userRepository.RefreshToken(refreshToken));
        }

    }
}
