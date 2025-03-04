﻿using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Pet_s_Land.Repositories.PaymentRepo;

namespace Pet_s_Land.Repositories
{
    public interface IPaymentRepo
    {
        Task<ResponseDto<string>> RazorOrderCreate(long price);
        Task<ResponseDto<bool>> RazorPayment(PaymentDto payment);
        Task<ResponseDto<bool>> CreateOrder(int userId, CreateOrderDto createOrderDTO);

        Task<ResponseDto<bool>> PlaceOrder(int userId, CreateOrderDto createOrderDTO);
        Task<ResponseDto<List<ViewOrderUserDetailDto>>> GetUserOrders(int userId);    
    }
    public class PaymentRepo : IPaymentRepo
    {
        private readonly AppDbContext _appDbContext;
        private const string RazorpayKey = "";
        private const string RazorpaySecret = "";

        public PaymentRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ResponseDto<string>> RazorOrderCreate(long price)
        {
            try
            {
                var client = new RazorpayClient(RazorpayKey, RazorpaySecret);
                var order = client.Order.Create(new Dictionary<string, object>
                {
                    { "amount", price * 100 },
                    { "currency", "INR" },
                    { "receipt", Guid.NewGuid().ToString() }
                });

                //var paymentOrder = new PaymentOrder
                //{
                var OrderId = order["id"].ToString();
                //    Amount = price,
                //    Status = "Created",
                //    CreatedAt = DateTime.UtcNow
                //};

                //await _appDbContext.PaymentOrders.AddAsync(paymentOrder);
                //await _appDbContext.SaveChangesAsync();

                return new ResponseDto<string>(OrderId, "Order created successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<string>(null, "Error creating Razorpay order: " + ex.Message, 400);
            }
        }

        public async Task<ResponseDto<bool>> RazorPayment(PaymentDto payment)
        {

            try
            {
                if (payment == null ||
                    string.IsNullOrEmpty(payment.RazorpayPaymentId) ||
                    string.IsNullOrEmpty(payment.RazorpayOrderId) ||
                    string.IsNullOrEmpty(payment.RazorpaySignature))
                {
                    return new ResponseDto<bool>(false, "Invalid payment details", 404);
                }

                string generatedSignature = GenerateSignature(payment.RazorpayPaymentId, payment.RazorpayOrderId);
                if (payment.RazorpaySignature == generatedSignature)
                {
                    return new ResponseDto<bool>(true, "Payment verified successfully", 200);
                }

                return new ResponseDto<bool>(false, "Payment verification failed", 400);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>(false, "Error verifying Razorpay payment: " + ex.Message, 400);
            }
        }


        private string GenerateSignature(string paymentId, string orderId)
        {
            using var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(RazorpaySecret));
            var hashBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(orderId + "|" + paymentId));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<ResponseDto<bool>> CreateOrder(int userId, CreateOrderDto createOrderDTO)
        {
            try
            {
                var cart = await _appDbContext.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(c => c.Product)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (cart == null)
                {
                    return new ResponseDto<bool>(false, "Cart is empty.", 400);
                }

                var order = new Models.OrderModels.Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Pending",
                    AddressId = createOrderDTO.AddressId,
                    TotalAmount = createOrderDTO.TotalAmount,
                    //OrderString = createOrderDTO.OrderString,
                    TransactionId = createOrderDTO.TransactionId,
                    OrderItems = cart.CartItems.Select(c => new OrderItem
                    {
                        Id = c.ProductId,
                        Quantity = c.Quantity,
                        TotalPrice = c.Quantity * c.Product.RP
                    }).ToList(),
                };

                // Check and Update Stock
                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
                    if (product != null && product.Stock < cartItem.Quantity)
                    {
                        return new ResponseDto<bool>(false, "Insufficient stock available", 400);
                    }
                    product.Stock -= cartItem.Quantity;
                }

                await _appDbContext.Orders.AddAsync(order);
                _appDbContext.Carts.Remove(cart);
                await _appDbContext.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Order placed successfully", 200);
            }
            catch (DbUpdateException ex)
            {
                return new ResponseDto<bool>(false, "Database update failed: " + ex.InnerException?.Message, 500);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>(false, "Error creating order: " + ex.Message, 500);
            }
        }


        public async Task<ResponseDto<bool>> PlaceOrder(int userId, CreateOrderDto createOrderDTO)
        {
            try
            {
                var address = await _appDbContext.Addresses.Where(a => a.UserId == userId && a.AddressId == createOrderDTO.AddressId).FirstOrDefaultAsync();


                if (address == null)
                {
                    return new ResponseDto<bool>(false, "Address did not match.", 400);
                }
                var cart = await _appDbContext.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                    return new ResponseDto<bool>(false, "Cart is empty.", 400);


                //  Calculate actual total price from cart items
                decimal actualTotalPrice = cart.CartItems.Sum(item => item.Quantity * item.Product.RP);

                //  Validate total price before proceeding
                if (actualTotalPrice != createOrderDTO.TotalAmount)
                {
                    return new ResponseDto<bool>(false, "Total price mismatch. Please refresh and try again.", 400);
                }



                // Check stock availability and deduct stock
                foreach (var item in cart.CartItems)
                {
                    if (item.Product.Stock < item.Quantity)
                        return new ResponseDto<bool>(false, $"Insufficient stock for product {item.ProductId}", 400);

                    item.Product.Stock -= item.Quantity;
                }

                var order = new Models.OrderModels.Order
                {
                    UserId = userId,
                    AddressId = createOrderDTO.AddressId,
                    TotalAmount = actualTotalPrice,
                    TransactionId = !string.IsNullOrEmpty(createOrderDTO.TransactionId) ? createOrderDTO.TransactionId : Guid.NewGuid().ToString(),
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "Pending"
                };

                _appDbContext.Orders.Add(order);
                await _appDbContext.SaveChangesAsync();  

                // Now assign OrderId to OrderItems and save them
                var orderItems = cart.CartItems.Select(item => new OrderItem
                {
                    OrderId = order.Id,  
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Unknown Product", // Store name separately
                    ProductImage = item.Product?.Image, // Store image separately
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * item.Product.RP
                }).ToList();

                await _appDbContext.OrderItems.AddRangeAsync(orderItems);
                _appDbContext.Carts.Remove(cart); 
                await _appDbContext.SaveChangesAsync();

                return new ResponseDto<bool>(true, "Order placed successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>(false, "Error creating order: " + ex.Message, 500);
            }
        }

        //public async Task<ResponseDto<List<ViewOrderUserDetailDto>>> GetUserOrders(int userId)
        //{
        //    try
        //    {
        //        var orders = await _appDbContext.Orders
        //            .Where(o => o.UserId == userId)
        //            .Include(o => o.OrderItems)
        //            .ThenInclude(oi => oi.Product)
        //            .ToListAsync();

        //        if (orders == null || orders.Count == 0)
        //        {
        //            return new ResponseDto<List<ViewOrderUserDetailDto>>(null, "No orders found", 404);
        //        }

        //        var orderDtos = orders.Select(order => new ViewOrderUserDetailDto
        //        {
        //            Id = order.Id,
        //            OrderDate = order.OrderDate,
        //            OrderStatus = order.OrderStatus.ToString(),
        //            TransactionId = order.TransactionId ?? "N/A",
        //            TotalPrice = order.OrderItems.Sum(oi => oi.TotalPrice),

        //            OrderProducts = order.OrderItems.Select(oi => new ViewOrderDto
        //            {
        //                Id = oi.ProductId ?? 0, // Ensures no null value
        //                ProductName = oi.Product != null ? oi.Product.Name : "Deleted Product",
        //                Image = oi.Product != null ? oi.Product.Image : "default-image.jpg",
        //                TotalAmount = oi.TotalPrice,
        //                Quantity = oi.Quantity,
        //            }).ToList()
        //        }).ToList(); // Converts IEnumerable to List

        //        return new ResponseDto<List<ViewOrderUserDetailDto>>(orderDtos, "Order details retrieved successfully", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto<List<ViewOrderUserDetailDto>>(null, "Internal Server Error: " + ex.Message, 500);
        //    }
        //}

        public async Task<ResponseDto<List<ViewOrderUserDetailDto>>> GetUserOrders(int userId)
        {
            try
            {
                var orders = await _appDbContext.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                    .Include(o => o.User) // Include User to access Name
                    .Include(o => o.Address) // Include Address to access details
                    .ToListAsync();

                if (orders == null || orders.Count == 0)
                {
                    return new ResponseDto<List<ViewOrderUserDetailDto>>(null, "No orders found", 404);
                }

                var orderDtos = orders.Select(order => new ViewOrderUserDetailDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    OrderStatus = order.OrderStatus.ToString(),
                    TransactionId = order.TransactionId ?? "N/A",
                    TotalPrice = order.OrderItems.Sum(oi => oi.TotalPrice),

                    CustomerName = order.User?.Name ?? "Unknown", // Fetch from User
                    PhoneNumber = order.Address?.PhoneNumber ?? "N/A", // Fetch from Address

                    // Basic required address details
                    AddressDetails = $"{order.Address?.HouseName}, {order.Address?.Place}, {order.Address?.PostOffice}",
                    Pincode = order.Address?.Pincode ?? "N/A",

                    OrderProducts = order.OrderItems.Select(oi => new ViewOrderDto
                    {
                        Id = oi.ProductId ?? 0,
                        ProductName = oi.ProductName,
                        Image = oi.ProductImage,
                        TotalAmount = oi.TotalPrice,
                        Quantity = oi.Quantity,
                    }).ToList()
                }).ToList();

                return new ResponseDto<List<ViewOrderUserDetailDto>>(orderDtos, "Order details retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ViewOrderUserDetailDto>>(null, "Internal Server Error: " + ex.Message, 500);
            }
        }



    }

}
    
