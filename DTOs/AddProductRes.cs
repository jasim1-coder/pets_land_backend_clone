namespace Pet_s_Land.DTOs
{
    public class AddProductRes
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal RP { get; set; }
        public decimal OldPrice { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; } // Changed IFormFile to string
        public List<string> Ingredients { get; set; }
        public string Seller { get; set; }
    }
}
