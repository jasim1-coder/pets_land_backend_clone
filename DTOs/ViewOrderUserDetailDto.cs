//namespace Pet_s_Land.DTOs
//{
//    public class ViewOrderUserDetailDto
//    {
//        public int Id { get; set; }
//        public DateTime OrderDate { get; set; }
//        public string OrderStatus { get; set; }
//        public string TransactionId { get; set; }
//        public decimal TotalPrice { get; set; }  

//        public List<ViewOrderDto> OrderProducts { get; set; }  
//    }

//}
namespace Pet_s_Land.DTOs
{
    public class ViewOrderUserDetailDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string TransactionId { get; set; }
        public decimal TotalPrice { get; set; }

        // New fields
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDetails { get; set; }
        public string Pincode { get; set; }

        public List<ViewOrderDto> OrderProducts { get; set; }
    }
}
