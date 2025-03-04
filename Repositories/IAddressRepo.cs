using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.AdressModels;

namespace Pet_s_Land.Repositories
{
    public interface IAddressRepo
    {    
        Task<ResponseDto<bool>> AddAddress(int userId, AddressCreateDto newAddress);
        Task<ResponseDto<bool>> RemoveAddress(int userId, int addressId);
        Task<ResponseDto<List<AddressResDto>>> GetAddresses(int userId);
    }

    public class AddressRepo : IAddressRepo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;


        public AddressRepo(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseDto<bool>> AddAddress(int userId, AddressCreateDto newAddress)
        {
            try
            {
                if (userId == null)
                {
                    throw new Exception("userId is not valid");
                }
                if (newAddress == null)
                {
                    throw new Exception("Address cannot be null");
                }
                var userAddress = await _appDbContext.Addresses
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
                if (userAddress.Count > 5)
                {
                    throw new Exception("Maximum limit of Address reached");
                }

                var address = new Address
                {
                    UserId = userId,
                    FullName = newAddress.FullName,
                    PhoneNumber = newAddress.PhoneNumber,
                    HouseName = newAddress.HouseName,
                    Place = newAddress.Place,
                    PostOffice = newAddress.PostOffice,
                    Pincode = newAddress.Pincode,
                    LandMark = newAddress.LandMark,

                };
                await _appDbContext.Addresses.AddAsync(address);
                await _appDbContext.SaveChangesAsync();
                return new ResponseDto<bool>(true, "Address added successfully.", 200);

            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>(false, ex.Message, 400);
            }
        }

        public async Task<ResponseDto<bool>> RemoveAddress(int userId, int addressId)
        {
            try
            {
                if (addressId == 0)
                {
                    return new ResponseDto<bool>(false, "Invalid address ID", 400);
                }

                // Check if the address exists
                var address = await _appDbContext.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.AddressId == addressId);

                if (address == null)
                {
                    return new ResponseDto<bool>(false, "No address available", 404);
                }

                // Check if the address is linked to any orders
                bool isAddressUsedInOrders = await _appDbContext.Orders
                    .AnyAsync(o => o.AddressId == addressId);

                if (isAddressUsedInOrders)
                {
                    return new ResponseDto<bool>(false, "Address cannot be deleted as it is linked to an order", 400);
                }

                _appDbContext.Addresses.Remove(address);
                await _appDbContext.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Address removed successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>(false, ex.Message, 500);
            }
        }


        public async Task<ResponseDto<List<AddressResDto>>> GetAddresses(int userId)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return new ResponseDto<List<AddressResDto>>(null, "User not found", 404);
                }

                var addresses = await _appDbContext.Addresses
                    .Where(u => u.UserId == userId)
                    .ToListAsync();

                if (!addresses.Any()) // Check if the list is empty
                {
                    return new ResponseDto<List<AddressResDto>>(null, "No addresses found for the user", 404);
                }

                var addressDtos = _mapper.Map<List<AddressResDto>>(addresses); // Correct Mapping
                return new ResponseDto<List<AddressResDto>>(addressDtos, "Addresses retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<AddressResDto>>(null, $"Error: {ex.Message}", 500);
            }
        }

    }
}
