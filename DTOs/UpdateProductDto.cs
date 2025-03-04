using System.ComponentModel.DataAnnotations;

namespace Pet_s_Land.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }  // Required for updating an existing product

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Retail Price must be greater than or equal to 0")]
        public decimal RP { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "MRP must be greater than or equal to 0")]
        public decimal MRP { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0")]
        public int Stock { get; set; }

        public IFormFile? Image { get; set; } // Image update is optional

        [Required(ErrorMessage = "Ingredients are required")]
        public List<string> Ingredients { get; set; }

        [Required(ErrorMessage = "Seller is required")]
        public string Seller { get; set; }
    }
}





