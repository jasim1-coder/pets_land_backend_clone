using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pet_s_Land.Models.ProductsModels;

namespace Pet_s_Land.Models.CategoryModels
{
    public class Category
    {

        public int CategoryId { get; set; }
        
        public string? CategoryName { get; set; }
        public virtual ICollection<Product>? Products { get; set; }

    }
}
