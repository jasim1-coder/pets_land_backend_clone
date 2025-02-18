using Pet_s_Land.Models.ProductsModels;
using Pet_s_Land.Models.UserModels;
using System.ComponentModel.DataAnnotations;

namespace Pet_s_Land.Models.WhishlistModel
{
    public class WishList
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public virtual User? Users { get; set; }
        public virtual Product? Products { get; set; }
    }
}
