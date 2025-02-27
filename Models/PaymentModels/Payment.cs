using Pet_s_Land.Models.OrderModels;

namespace Pet_s_Land.Models.PaymentModels
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
