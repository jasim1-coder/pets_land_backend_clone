using System.Collections.Generic;
using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IProductsServices
    {
        Task<ResponseDto<AddProductRes>> AddProductAsync(AddProductDto productdata);
        Task<ResponseDto<List<ProductSearchDto>>> GetAllProductsAsync();

        Task<ResponseDto<ProductDto>> GetProductByIdAsync(int Id);

        Task<ResponseDto<List<ProductDto>>> GetProductByCategryAsync(int Category);

        Task<ResponseDto<List<ProductDto>>> GetProductsByPaginatedAsync(int pageNum, int pageSize);

        Task<ResponseDto<List<ProductSearchDto>>> SearchProductsAsync(string searchTerm);

    }

    public class ProductsServices : IProductsServices
    {
        private readonly IProductsRepo _productsRepo;
        private readonly IMapper _mapper;

        public ProductsServices(IProductsRepo productsRepo, IMapper mapper)
        {
            _productsRepo = productsRepo;
            _mapper = mapper;
        }

        public async Task<ResponseDto<AddProductRes>> AddProductAsync(AddProductDto productdata)
        {
            return await _productsRepo.AddProductAsync(productdata);
        }
        public async Task<ResponseDto<List<ProductSearchDto>>> GetAllProductsAsync()

        {
            return await _productsRepo.GetAllProductsAsync();
        }


        public async Task<ResponseDto<ProductDto>> GetProductByIdAsync(int Id)
        {
            return await _productsRepo.GetProductByIdAsync(Id);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetProductByCategryAsync(int Category)
        {
            return await _productsRepo.GetProductByCategryAsync(Category);

        }

        public async  Task<ResponseDto<List<ProductDto>>> GetProductsByPaginatedAsync(int pageNum, int pageSize)
        {
            return await _productsRepo.GetProductsByPaginatedAsync(pageNum, pageSize);
        }

        public async Task<ResponseDto<List<ProductSearchDto>>> SearchProductsAsync(string searchTerm)
        {
            return await _productsRepo.SearchProductsAsync(searchTerm);
        }

    }
}
