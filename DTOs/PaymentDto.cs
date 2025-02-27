namespace Pet_s_Land.DTOs
{
    public class PaymentDto
    {
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpayOrderId { get; set; }
        public string? RazorpaySignature { get; set; }
    }
}
