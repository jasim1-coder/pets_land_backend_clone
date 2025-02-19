namespace Pet_s_Land.DTOs
{
    public class CartResDto
    {
        public List<CartViewDto> cartItemsperUser { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
