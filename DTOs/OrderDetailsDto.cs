using Microsoft.EntityFrameworkCore;
using Pet_s_Land.DTOs;
using Pet_s_Land.Enums;

namespace Pet_s_Land.DTOs
{
    //public class OrderDto
    //{
    //    public int Id { get; set; }
    //    public DateTime OrderDate { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public OrderStatusEnum OrderStatus { get; set; }  // Change from string to enum ✅
    //    public string CustomerName { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    //}

    //public class OrderItemDto
    //{
    //    public string ProductName { get; set; }
    //    public string ProductImage { get; set; }

    //    public int Quantity { get; set; }


    //}


    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }

        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; } // Full address from Address entity

        public string PaymentMethod { get; set; } = "Razorpay"; // Hardcoded payment method
        public int? ModifiedByAdminId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } // Product Price
    }



}


