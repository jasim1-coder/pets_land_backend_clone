using AutoMapper;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Repositories;

namespace Pet_s_Land.Servies
{
    public interface IProductsServices
    {
        Task AddProductAsync(Product product);
        //Task<List<Product>> GetAllProductsAsync();
    }

    public class ProductsServices : IProductsServices
    {
        private readonly IUserRepoRegister _userRepoRegister;
        private readonly IMapper _mapper;

        public ProductsServices(IUserRepoRegister userRepoRegister, IMapper mapper)
        {
            _userRepoRegister = userRepoRegister;
            _mapper = mapper;
        }

        public async Task<ResponseDto<object>> AddProductAsync(ProductDto productdata)
        {
            return await _userRepoRegister.AddProductAsync(productdata);
        }




    }
}
