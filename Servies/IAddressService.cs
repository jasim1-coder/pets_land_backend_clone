using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IAddressService
    {
        Task<ResponseDto<bool>> AddAddress(int userId, AddressCreateDto newAddress);
        Task<ResponseDto<bool>> RemoveAddress(int userId, int addressId);
        Task<ResponseDto<List<AddressResDto>>> GetAddresses(int userId);
    }
    public class AddressService : IAddressService
    {
        private readonly IAddressRepo _addressRepo;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepo addressRepo, IMapper mapper)
        {
            _addressRepo = addressRepo;
            _mapper = mapper;
        }

        public async Task<ResponseDto<bool>> AddAddress(int userId, AddressCreateDto newAddress)
        {
            return await _addressRepo.AddAddress(userId,newAddress);
        }

        public async Task<ResponseDto<bool>> RemoveAddress(int userId, int addressId)
        {
            return await _addressRepo.RemoveAddress(userId, addressId);
        }

        public async Task<ResponseDto<List<AddressResDto>>> GetAddresses(int userId)
        {
            return await _addressRepo.GetAddresses(userId);
        }

    }
}
