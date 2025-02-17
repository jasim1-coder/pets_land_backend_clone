using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IProductsServices
    {
        Task<ResponseDto<object>> AddProductAsync(ProductDto productdata);
        //Task<List<Product>> GetAllProductsAsync();
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




    }
}
