namespace Pet_s_Land.DTOs
{
    public class WishListResDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
    }
}
