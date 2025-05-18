using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Pet_s_Land.DTOs
{
    public class AddProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public decimal? RP { get; set; }
        public decimal? MRP { get; set; }
        public int? Stock { get; set; }
        public IFormFile? Image { get; set; }
        public List<string>? Ingredients { get; set; }
        public string? Seller { get; set; }
    }

}
