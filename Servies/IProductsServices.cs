using System.Collections.Generic;
using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IProductsServices
    {
        Task<ResponseDto<object>> AddProductAsync(ProductDto productdata);
        Task<ResponseDto<List<Product>>> GetAllProductsAsync();

        Task<ResponseDto<Product>> GetProductByIdAsync(int Id);

        Task<ResponseDto<List<Product>>> GetProductByCategryAsync(string Category);

        Task<ResponseDto<List<Product>>> GetProductsByPaginatedAsync(int pageNum, int pageSize);

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

        public async Task<ResponseDto<object>> AddProductAsync(ProductDto productdata)
        {
            return await _productsRepo.AddProductAsync(productdata);
        }
        public async Task<ResponseDto<List<Product>>> GetAllProductsAsync()

        {
            return await _productsRepo.GetAllProductsAsync();
        }


        public async Task<ResponseDto<Product>> GetProductByIdAsync(int Id)
        {
            return await _productsRepo.GetProductByIdAsync(Id);
        }

        public async Task<ResponseDto<List<Product>>> GetProductByCategryAsync(string Category)
        {
            return await _productsRepo.GetProductByCategryAsync(Category);

        }

        public async  Task<ResponseDto<List<Product>>> GetProductsByPaginatedAsync(int pageNum, int pageSize)
        {
            return await _productsRepo.GetProductsByPaginatedAsync(pageNum, pageSize);
        }


    }
}
