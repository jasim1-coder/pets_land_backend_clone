using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IRegisterUser
    {
        Task<ResponseDto<object>> RegisterUser(UserDto regdata);

    }
    public class RegisterUsers : IRegisterUser
    {
        private readonly IUserRepoRegister _userRepository;
        private readonly IMapper _mapper;

        public RegisterUsers(IUserRepoRegister userRepository, IMapper mapper)
        {
            userRepository = _userRepository;
            mapper = _mapper;
        }
        public async Task<ResponseDto<object>> RegisterUser(UserDto regdata)
        {
            return await _userRepository.RegisterUser(regdata);
        }

    }
}
