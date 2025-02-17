using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Repositories;
using Pet_s_Land.Services;
using Pet_s_Land.Servies;

namespace Pet_s_Land.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepo _productsRepo;
        private readonly IProductsServices _productsServices;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(
            IProductsRepo productsRepo,
            IProductsServices productsServices,
            IMapper mapper,
            ICloudinaryService cloudinaryService)
        {
            _productsRepo = productsRepo;
            _productsServices = productsServices;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productData)
        {
            if (productData.Image == null || productData.Image.Length == 0)
            {
                return BadRequest(new { message = "No image uploaded." });
            }

            // Upload image to Cloudinary
            var imageUrl = await _cloudinaryService.UploadImageAsync(productData.Image);

            // Mapping product data from DTO to Model
            var product = _mapper.Map<Product>(productData);
            product.Image = imageUrl; // Store Cloudinary URL in the database

            // Save product to database through the service
            var result = await _productsRepo.AddProductAsync(productData);

            return Ok(new { message = "Product added successfully", product = result });
        }

        [HttpGet("Get all product")]
        public async Task<ResponseDto<List<Product>>> GetAllProductsAsync()
        {
            return await _productsRepo.GetAllProductsAsync();
            
            //return StatusCode(result.StatusCode,result);
           
        }

    }
}
