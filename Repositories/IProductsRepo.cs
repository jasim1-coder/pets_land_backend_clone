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
        Task<ResponseDto<AddProductRes>> AddProductAsync(AddProductDto productData);
        Task<ResponseDto<List<ProductSearchDto>>> GetAllProductsAsync();

        Task<ResponseDto<ProductDto>> GetProductByIdAsync(int Id);
        Task<ResponseDto<List<ProductDto>>> GetProductByCategryAsync(int CategoryId);

        Task<ResponseDto<List<ProductDto>>> GetProductsByPaginatedAsync(int pageNum, int pageSize);

        Task<ResponseDto<List<ProductSearchDto>>> SearchProductsAsync(string searchTerm);


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

        public async Task<ResponseDto<AddProductRes>> AddProductAsync(AddProductDto productData)
        {
            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(productData.Image);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return new ResponseDto<AddProductRes>(null, "Image upload failed", 500);
                }

                var product = _mapper.Map<Product>(productData);
                product.Image = imageUrl;
                _appDbContext.Products.Add(product);
                await _appDbContext.SaveChangesAsync();

                var resProduct = _mapper.Map<AddProductRes>(product);
                return new ResponseDto<AddProductRes>(resProduct, "Product added successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<AddProductRes>(null, ex.Message, 500);
            }
        }

        public async Task<ResponseDto<List<ProductSearchDto>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _appDbContext.Products
                    .Select(p => new ProductSearchDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        RP = p.RP,
                        MRP = p.MRP,
                        ImageUrl = p.Image,
                        Category = p.Category.CategoryName,
                        Seller = p.Seller,
                        Stock = p.Stock,
                        Description = p.Description,
                        Ingredients = p.Ingredients // Include Ingredients
                    })
                    .ToListAsync();

                return products.Any()
                    ? new ResponseDto<List<ProductSearchDto>>(products, "Success", 200)
                    : new ResponseDto<List<ProductSearchDto>>(null, "No products available", 404);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ProductSearchDto>>(null, ex.Message, 500);
            } 
        }


        public async Task<ResponseDto<ProductDto>> GetProductByIdAsync(int Id)
        {
            try
            {
                var product = await _appDbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == Id);
                var resultproduct = _mapper.Map<ProductDto>(product);
                return resultproduct != null
                    ? new ResponseDto<ProductDto>(resultproduct, "Product retrieved successfully", 200)
                    : new ResponseDto<ProductDto>(null, "No item with such Id in Table", 404);
            }
            catch (Exception ex) 
            {
                return new ResponseDto<ProductDto>(null, ex.Message, 500);

            }
        }


        public async Task<ResponseDto<List<ProductDto>>> GetProductByCategryAsync(int CategoryId)
        {
            try
            {
                var products = await _appDbContext.Products.Include(p => p.Category)
                    .Where(p => p.Category.CategoryId == CategoryId)
                    .Select(p => _mapper.Map<ProductDto>(p))
                    .ToListAsync();

                return products.Any()
                    ? new ResponseDto<List<ProductDto>>(products, "List of products in this category", 200)
                    : new ResponseDto<List<ProductDto>>(null, "No items in such category", 404);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ProductDto>>(null, ex.Message, 500);
            }
        }



        public async Task<ResponseDto<List<ProductDto>>> GetProductsByPaginatedAsync(int pageNum, int pageSize)
        {
            try
            {
                var skip = (pageNum - 1) * pageSize;
                var products = await _appDbContext.Products.Include(p => p.Category).Skip(skip).Take(pageSize).ToListAsync();

                var productDtos = _mapper.Map<List<ProductDto>>(products); 


                return productDtos.Any()
                    ? new ResponseDto<List<ProductDto>>(productDtos, "List of products", 200)
                    : new ResponseDto<List<ProductDto>>(null, "No products found", 404);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ProductDto>>(null, ex.Message, 500);
            }
        }
        public async Task<ResponseDto<List<ProductSearchDto>>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new ResponseDto<List<ProductSearchDto>>(null, "Search term cannot be empty", 400);
                }

                var products = await _appDbContext.Products
                    .Where(p => p.Name.Contains(searchTerm) || p.Category.CategoryName.Contains(searchTerm))
                    .Select(p => new ProductSearchDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        RP = p.RP,
                        MRP = p.MRP,
                        ImageUrl = p.Image,
                        Category = p.Category.CategoryName,
                        Seller = p.Seller,
                        Stock = p.Stock,
                        Description = p.Description,
                        Ingredients = p.Ingredients
                    })
                    .ToListAsync();


                return products.Any()
                    ? new ResponseDto<List<ProductSearchDto>>(products, "Success", 200)
                    : new ResponseDto<List<ProductSearchDto>>(null, "No products found", 404);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ProductSearchDto>>(null, ex.Message, 500);
            }
        }

    }
} 