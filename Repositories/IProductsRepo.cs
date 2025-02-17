using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Services;
using System.Threading.Tasks;

namespace Pet_s_Land.Repositories
{
    public interface IProductsRepo
    {
        Task<ResponseDto<object>> AddProductAsync(ProductDto productData);
        //Task<List<Product>> GetAllProductsAsync();
    }

    public class ProductsRepo : IProductsRepo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsRepo(AppDbContext appDbContext, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ResponseDto<object>> AddProductAsync(ProductDto productData)
        {
            // Upload the image using CloudinaryService
            var imageUrl = await _cloudinaryService.UploadImageAsync(productData.Image);

            if (string.IsNullOrEmpty(imageUrl))
            {
                return new ResponseDto<object> ( productData, "Image upload failed",500 );
            }

            var product = _mapper.Map<Product>(productData);
            product.Image = imageUrl;

            _appDbContext.Products.Add(product);
            await _appDbContext.SaveChangesAsync();

            return new ResponseDto<object> (productData,"Product added successfully",200 );
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _appDbContext.Products.ToListAsync();
        }
    }
}
