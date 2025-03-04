namespace Pet_s_Land.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal RP { get; set; }
        public decimal MRP { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string Seller { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
    }
}
