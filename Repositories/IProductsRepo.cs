using AutoMapper;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;

namespace Pet_s_Land.Repositories
{
    public interface IProductsRepo
    {
        Task<ResponseDto<object>> AddProductAsync(ProductDto productData);

    }

    public class ProductsRepo : IProductsRepo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ProductsRepo(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

            public async Task<ResponseDto<object>> AddProductAsync(ProductDto productData)
        {
            

        }


        }       
    }
