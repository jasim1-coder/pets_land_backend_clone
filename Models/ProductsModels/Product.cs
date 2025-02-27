
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pet_s_Land.Models.CartModels;
using Pet_s_Land.Models.CategoryModels;

namespace Pet_s_Land.Models.ProductsModels
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Product name is required")]

        public string Name { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        [Column(TypeName = "decimal(18,2)")]

        public decimal Price { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Old Price must be greater than or equal to 0")]
        [Column(TypeName = "decimal(18,2)")]

        public decimal OldPrice { get; set; }
        public string Image { get; set; }


        public int? CategoryId { get; set; }
        [Required]


        public string Seller { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal 0")]

        public int Stock { get; set; }
        [Required]

        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
        public virtual List<CartItem> CartItems { get; set; }
        public virtual Category? Category { get; set; }



    }
}
