using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IProductsServices _productsServices;

        public ProductsController(IProductsServices productsServices)
        {
            _productsServices = productsServices;
        }


        [Authorize(Roles = "Admin")]

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct(AddProductDto productData)
        {
            if (productData.Image == null || productData.Image.Length == 0)
            {
                return BadRequest(new { message = "No image uploaded." });
            }

            var result = await _productsServices.AddProductAsync(productData);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var result = await _productsServices.GetAllProductsAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductByIdAsync(int Id)
        {
            var result = await _productsServices.GetProductByIdAsync(Id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetProductByCategory")]
        public async Task<IActionResult> GetProductByCategryAsync(int CategoryId)
        {
            var result = await _productsServices.GetProductByCategryAsync(CategoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetProductPaginated")]
        public async Task<IActionResult> GetProductPaginatedAsync(int pageNum, int pageSize)
        {
            var result = await _productsServices.GetProductsByPaginatedAsync(pageNum, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            var response = await _productsServices.SearchProductsAsync(query);
            return StatusCode(response.StatusCode, response);
        }

    }
}
