using Pet_s_Land.Models.ProductsModels;

namespace Pet_s_Land.Models.OrderModels
{
    public class OrderItem
    {   
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public string? ProductName { get; set; } 
        public string? ProductImage { get; set; }


        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } 
    }

}
