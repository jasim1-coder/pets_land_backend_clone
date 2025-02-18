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
        Task<ResponseDto<List<Product>>> GetAllProductsAsync();

        Task<ResponseDto<Product>> GetProductByIdAsync(int Id);
        Task<ResponseDto<List<Product>>> GetProductByCategryAsync(string Category);

        Task<ResponseDto<List<Product>>> GetProductsByPaginatedAsync(int pageNum, int pageSize);


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
                return new ResponseDto<object>(productData, "Image upload failed", 500);
            }

            var product = _mapper.Map<Product>(productData);


            product.Image = imageUrl;

            _appDbContext.Products.Add(product);
            await _appDbContext.SaveChangesAsync();

            return new ResponseDto<object>(productData, "Product added successfully", 200);
        }

        public async Task<ResponseDto<List<Product>>> GetAllProductsAsync()
        {
            var result = await _appDbContext.Products.ToListAsync();
            if (result.Count > 0)
            {
                return new ResponseDto<List<Product>>(result, "List of all Products", 200);
            }
            else
            {
                return new ResponseDto<List<Product>>(null, "No items in Table", 404);
            }

        }

        public async Task<ResponseDto<Product>> GetProductByIdAsync(int Id)
        {

            var result = await _appDbContext.Products.FirstOrDefaultAsync(product => product.Id == Id);

            if (result != null)
            {
                return new ResponseDto<Product>(result, "Product by Id", 200);
            }
            else
            {
                return new ResponseDto<Product>(null, "No item wtih such Id in Table", 404);
            }

        }


        public async Task<ResponseDto<List<Product>>> GetProductByCategryAsync(string Category)
        {
            var result = await _appDbContext.Products.Where(product => product.Category.ToLower() == Category.ToLower()).ToListAsync();
            if (result != null)
            {
                return new ResponseDto<List<Product>>(result, "List of product in this category", 200);
            }
            else
            {
                return new ResponseDto<List<Product>>(null, "No items in such category", 404);
            }
        }



        public async Task<ResponseDto<List<Product>>> GetProductsByPaginatedAsync(int pageNum, int pageSize)
        {
            var skip = (pageNum - 1) * pageSize;

            var result = await _appDbContext.Products.Skip(skip).Take(pageSize).ToListAsync();
            //var totalItems = await _appDbContext.Products.CountAsync();
            //var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (result.Count > 0)
            {
                //var paginationInfo = new
                //{
                //    PageNumber = pageNumber,
                //    PageSize = pageSize,
                //    TotalItems = totalItems,
                //    TotalPages = totalPages
                //};
                

                    return new ResponseDto<List<Product>>(result, "List of products", 200);

                }


                 else
                {
                    return new ResponseDto<List<Product>>(null, "No products found", 404);
                }


            }



        }
    } 