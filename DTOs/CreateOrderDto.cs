namespace Pet_s_Land.DTOs
{
    public class CreateOrderDto
    {
        public int AddressId { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionId { get; set; }
    }
}
