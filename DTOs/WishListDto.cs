using System.ComponentModel.DataAnnotations;

namespace Pet_s_Land.DTOs
{
    public class WishListDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
