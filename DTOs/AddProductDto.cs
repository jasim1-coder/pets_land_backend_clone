using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Pet_s_Land.DTOs
{
    public class AddProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal RP { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "MRP must be greater than or equal to 0")]
        public decimal MRP { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Product image is required")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Ingredients are required")]
        public List<string> Ingredients { get; set; }

        [Required(ErrorMessage = "Seller is required")]
        public string Seller { get; set; }
    }
}
