using System.Collections.Generic;
using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.OrderModels;
using Pet_s_Land.Models.UserModels;
using Pet_s_Land.Repositories;
using static Pet_s_Land.DTOs.UserViewDto;

namespace Pet_s_Land.Servies
{
    public interface IAdminServices
    {
        Task<ResponseDto<List<UserViewDTO>>> GetAllUser();
        Task<ResponseDto<UserViewDTO>>GetUserById(int id);

        Task<ResponseDto<bool>>BlockOrUnblockUser(int id);

        Task<ResponseDto<int>> TotalProductPurchased();
        Task<ResponseDto<decimal>> GetTotalRevenue();

        Task<ResponseDto<List<OrderDto>>> GetAllOrdersWithItems();

        Task<ResponseDto<string>> DeleteProduct(int productId);

        Task<ResponseDto<AddProductRes>> UpdateProductAsync(int productId, AddProductDto productData);






    }
    public class AdminServices : IAdminServices
    {
        private readonly IAdminRepo _adminRepo;

        public AdminServices(IAdminRepo adminRepo)
        {
            _adminRepo = adminRepo;
        }

       public async Task<ResponseDto<List<UserViewDTO>>> GetAllUser()
        {
            return await _adminRepo.GetAllUser();
        }

        public async Task<ResponseDto<UserViewDTO>>GetUserById(int id)
        {
            return await _adminRepo.GetUserById(id);
        }

        public async Task<ResponseDto<bool>> BlockOrUnblockUser(int id)
        {
            return await _adminRepo.BlockOrUnblockUser(id);
        }

        public async Task<ResponseDto<int>> TotalProductPurchased()
        {
            return await _adminRepo.TotalProductPurchased();
        }

        public async Task<ResponseDto<decimal>> GetTotalRevenue()
        {
            return await _adminRepo.GetTotalRevenue();
        }
        public async Task<ResponseDto<List<OrderDto>>> GetAllOrdersWithItems()
        {
            return await _adminRepo.GetAllOrdersWithItems();
        }

        public async Task<ResponseDto<string>> DeleteProduct(int productId)
        {
            return await _adminRepo.DeleteProduct(productId);
        }

        public async Task<ResponseDto<AddProductRes>> UpdateProductAsync(int productId, AddProductDto productData)
        {
            return await _adminRepo.UpdateProductAsync(productId, productData);

        }

}


}
