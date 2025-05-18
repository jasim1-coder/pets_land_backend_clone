﻿using Pet_s_Land.Enums;
using Pet_s_Land.Models.AdressModels;
using Pet_s_Land.Models.UserModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Pet_s_Land.Models.OrderModels
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow; 

        [Required]
        [ForeignKey("Address")]
        public int AddressId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal TotalAmount { get; set; }

        [Required]

        public OrderStatusEnum OrderStatus { get; set; }

        public string? OrderString { get; set; } 
        public string? TransactionId { get; set; }

        public int? ModifiedByAdminId { get; set; } // Stores the Admin ID who modified the order
        public DateTime? ModifiedDate { get; set; } // Stores the modification date
        public virtual User? User { get; set; }
        public virtual Address? Address { get; set; }
        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
