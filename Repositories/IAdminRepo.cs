﻿using AutoMapper;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using Pet_s_Land.Enums;
using Pet_s_Land.Models.OrderModels;
using Pet_s_Land.Models.UserModels;
using Pet_s_Land.Services;
using static Pet_s_Land.DTOs.UserViewDto;

namespace Pet_s_Land.Repositories
{
    public interface IAdminRepo
    {
        Task<ResponseDto<List<UserViewDTO>>> GetAllUser();
        Task<ResponseDto<UserViewDTO>> GetUserById(int id);

        Task<ResponseDto<bool>> BlockOrUnblockUser(int id);

        Task<ResponseDto<int>> TotalProductPurchased();

        Task<ResponseDto<decimal>> GetTotalRevenue();

        Task<ResponseDto<List<OrderDto>>> GetAllOrdersWithItems();
        Task<ResponseDto<List<OrderDto>>> GetUserOrders(int userId);

        Task<ResponseDto<List<OrderDto>>> FilterOrderDetails(int? userId, OrderStatusEnum? status, DateTime? startDate, DateTime? endDate);

        Task<ResponseDto<string>> UpdateOrderStatus(int orderId, OrderStatusEnum newStatus, int adminId);



        Task<ResponseDto<string>> DeleteProduct(int productId);

        Task<ResponseDto<AddProductRes>> UpdateProductAsync(int productId, AddProductDto productData);






    }

    public class AdminRepo : IAdminRepo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinary;

        public AdminRepo(AppDbContext appDbContext, IMapper mapper, ICloudinaryService cloudinary) {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<ResponseDto<List<UserViewDTO>>> GetAllUser()
        {
            try
            {
                var user = await _appDbContext.Users.Where(u => u.Role == "User").ToListAsync();
                if (!user.Any()) {
                    return new ResponseDto<List<UserViewDTO>>(null, "No users found", 400);
                }

                var userDtos = _mapper.Map<List<UserViewDTO>>(user);
                return new ResponseDto<List<UserViewDTO>>(userDtos, "Users retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<UserViewDTO>>(null, "Internal Server Error Occured", 500);

            }
        }

        public async Task<ResponseDto<UserViewDTO>> GetUserById(int id)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return new ResponseDto<UserViewDTO>(null, "No users found", 400);
                }

                // Get total items in the cart
                int totalCartItems = await _appDbContext.CartItems
                    .Where(ci => ci.Cart.UserId == id)
                    .SumAsync(ci => ci.Quantity);

                // Get total number of orders
                int totalOrders = await _appDbContext.Orders
                    .CountAsync(o => o.UserId == id);

                // Map user to DTO
                var userdto = _mapper.Map<UserViewDTO>(user);

                // Set additional fields
                userdto.TotalCartItems = totalCartItems;
                userdto.TotalOrders = totalOrders;

                return new ResponseDto<UserViewDTO>(userdto, "User retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<UserViewDTO>(null, "Internal Server Error Occurred", 500);
            }
        }


        public async Task<ResponseDto<bool>> BlockOrUnblockUser(int id)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return new ResponseDto<bool>(false, "No users found", 400);
                }
                user.IsBlocked = !user.IsBlocked;
                _appDbContext.SaveChanges();

                return user.IsBlocked == true ? new ResponseDto<bool>(true, "User Blocked Successfully", 200) : new ResponseDto<bool>(true, "User UnBlocked Successfully", 200);

            }
            catch (Exception ex)
            {
                return new ResponseDto<bool>(false, "Internal Server Error Occured", 500);

            }
        }


        public async Task<ResponseDto<int>> TotalProductPurchased()
        {
            try
            {
                var result = await _appDbContext.OrderItems.SumAsync(oi => oi.Quantity);

                if (result == 0)
                {
                    return new ResponseDto<int>(0, "No products have been purchased yet.",204);
                }

                return new ResponseDto<int>(result, "Total product quantity retrieved successfully.",200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<int>(0, "Internal Server Error Occurred: " + ex.Message, 500);
            }
        }

        public async Task<ResponseDto<decimal>> GetTotalRevenue()
        {
            try
            {
                var result = await _appDbContext.Orders.SumAsync(o => o.TotalAmount);
                if (result == 0)
                {
                    return new ResponseDto<decimal>(0, "No revenue generated yet.", 204);
                }

                return new ResponseDto<decimal>(result, "Total revenue retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<decimal>(0, "Internal Server Error Occurred: " + ex.Message, 500);
            }
        }

        public async Task<ResponseDto<List<OrderDto>>> GetAllOrdersWithItems()
        {
            try
            {
                var orders = await _appDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.Address)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product) // Include Product details for reference
                    .ToListAsync();

                if (!orders.Any())
                {
                    return new ResponseDto<List<OrderDto>>(null, "No orders found.", 204);
                }

                var orderDtos = orders.Select(order => new OrderDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    OrderStatus = order.OrderStatus,

                    CustomerName = order.User?.Name ?? "Unknown",
                    PhoneNumber = order.Address?.PhoneNumber ?? "N/A",
                    Address = order.Address != null
                        ? $"{order.Address.HouseName}, {order.Address.PostOffice}, {order.Address.Place}, {order.Address.Pincode}"
                        : "Address not available",

                    PaymentMethod = "Razorpay", // Hardcoded for now
                    ModifiedByAdminId = order.ModifiedByAdminId,
                    ModifiedDate = order.ModifiedDate ?? order.OrderDate,

                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.ProductName ?? "Unknown Product", // Use stored product name
                        ProductImage = oi.ProductImage ?? "", // Use stored product image
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * (oi.Product?.RP ?? 0) // Calculate price * quantity
                    }).ToList()
                }).ToList();

                return new ResponseDto<List<OrderDto>>(orderDtos, "Orders retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<OrderDto>>(null, "Internal Server Error: " + ex.Message, 500);
            }
        }



        //public async Task<ResponseDto<string>> DeleteProduct(int productId)
        //{
        //    try
        //    {
        //        var product = await _appDbContext.Products.FindAsync(productId);

        //        if (product == null)
        //        {
        //            return new ResponseDto<string>(null, "Product not found.", 404);
        //        }

        //        // Remove product from cart and wishlist
        //        var cartItems = _appDbContext.CartItems.Where(ci => ci.ProductId == productId);
        //        var wishlistItems = _appDbContext.WishLists.Where(wl => wl.ProductId == productId);

        //        _appDbContext.CartItems.RemoveRange(cartItems);
        //        _appDbContext.WishLists.RemoveRange(wishlistItems);


        //        product.IsDeleted = true;
        //        await _appDbContext.SaveChangesAsync();

        //        var orderItems = await _appDbContext.OrderItems.Where(oi => oi.ProductId == productId).ToListAsync();
        //        foreach (var orderItem in orderItems)
        //        {
        //            orderItem.ProductId = null;
        //        }

        //        await _appDbContext.SaveChangesAsync();

        //        return new ResponseDto<string>("Product soft deleted successfully, and removed from cart & wishlist.", "Success", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto<string>(null, "Error deleting product: " + ex.Message, 500);
        //    }
        //}


        //public async Task<ResponseDto<List<OrderDto>>> GetUserOrders(int userId)
        //{
        //    try
        //    {
        //        if (userId <= 0)
        //        {
        //            return new ResponseDto<List<OrderDto>>(null, "Invalid user ID.", 400);
        //        }
        //        var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //        if (user == null)
        //        {
        //            return new ResponseDto<List<OrderDto>>(null, "No users found", 400);
        //        }

        //        var userOrders = await _appDbContext.Orders
        //            .Where(o => o.UserId == userId)
        //            .Include(o => o.OrderItems)
        //            .ThenInclude(oi => oi.Product) // Ensuring Product details are included
        //            .Select(o => new OrderDto
        //            {
        //                Id = o.Id,
        //                OrderDate = o.OrderDate,
        //                TotalAmount = o.TotalAmount,
        //                OrderStatus = o.OrderStatus,
        //                CustomerName = o.User.Name,
        //                PhoneNumber = o.User.PhoneNumber,
        //                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
        //                {
        //                    ProductName = oi.Product.Name,
        //                    ProductImage = oi.Product.Image,
        //                    Quantity = oi.Quantity
        //                }).ToList()
        //            })
        //            .ToListAsync();

        //        if (userOrders == null || userOrders.Count == 0)
        //        {
        //            return new ResponseDto<List<OrderDto>>(null, "No orders found for the user.", 404);
        //        }

        //        return new ResponseDto<List<OrderDto>>(userOrders, "Orders fetched successfully.", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.LogError($"Error retrieving user orders: {ex.Message}");
        //        return new ResponseDto<List<OrderDto>>(null, "An error occurred while fetching orders.", 500);
        //    }

        //}


        public async Task<ResponseDto<List<OrderDto>>> GetUserOrders(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new ResponseDto<List<OrderDto>>(null, "Invalid user ID.", 400);
                }

                var user = await _appDbContext.Users
                    .Include(u => u.Addresses) // Include Address details
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new ResponseDto<List<OrderDto>>(null, "No users found", 400);
                }

                var userOrders = await _appDbContext.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product) // Ensuring Product details are included
                        .Include(o => o.User)
                        .Include(o => o.Address) // Include address details for orders
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        OrderStatus = o.OrderStatus,

                        CustomerName = user.Name,
                        PhoneNumber = o.Address != null ? o.Address.PhoneNumber : "N/A",
                        //Email = o.Address != null ? o.Address.Email : "N/A",
                        Address = o.Address != null
                            ? $"{o.Address.HouseName}, {o.Address.PostOffice}, {o.Address.Place}, {o.Address.Pincode}"
                            : "Address not available",

                        PaymentMethod = "Razorpay", // Hardcoded for now
                        ModifiedByAdminId = o.ModifiedByAdminId,
                        ModifiedDate = o.ModifiedDate ?? o.OrderDate,

                        OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                        {
                            ProductName = oi.ProductName ?? "Unknown Product",
                            ProductImage = oi.ProductImage ?? "",
                            Quantity = oi.Quantity,
                            TotalPrice = oi.TotalPrice // Product price
                        }).ToList()
                    })
                    .ToListAsync();

                if (userOrders == null)
                {
                    return new ResponseDto<List<OrderDto>>(null, "No orders found for the user.", 404);
                }

                return new ResponseDto<List<OrderDto>>(userOrders, "Orders fetched successfully.", 200);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error retrieving user orders: {ex.Message}");
                return new ResponseDto<List<OrderDto>>(null, "An error occurred while fetching orders.", 500);
            }
        }


        public async Task<ResponseDto<List<OrderDto>>> FilterOrderDetails(int? userId,OrderStatusEnum? status,DateTime? startDate,DateTime? endDate)
        {
            try
            {
                var query = _appDbContext.Orders
                    .Include(o => o.User)
                     .Include(o => o.OrderItems)
                    // Include user details
                    .ThenInclude(oi => oi.Product) // Ensuring Product details are included
                    .Include(o => o.Address)
                    .Include(o => o.OrderItems) // Include order items
                    .AsQueryable();

                // Apply filters
                if (userId.HasValue)
                    query = query.Where(o => o.UserId == userId.Value);

                if (status.HasValue)
                    query = query.Where(o => o.OrderStatus == status.Value);

                if (startDate.HasValue && endDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate.Date >= startDate.Value.Date &&
                                             o.OrderDate.Date <= endDate.Value.Date);
                }
                else if (startDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate.Date >= startDate.Value.Date);
                }
                else if (endDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate.Date <= endDate.Value.Date);
                }


                var orders = await query.ToListAsync();

                if (!orders.Any())
                    return new ResponseDto<List<OrderDto>>([], "No matching orders found.", 400);

                // Convert Order -> OrderDto
                var orderDtos = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    CustomerName = o.User?.Name ?? "Unknown",
                    PhoneNumber = o.User?.PhoneNumber ?? "N/A",
                    Address = o.Address != null
                ? $"{o.Address.HouseName}, {o.Address.PostOffice}, {o.Address.Place}, {o.Address.Pincode}"
                : "Address not available",
                    PaymentMethod = "Razorpay", // Hardcoded for now
                    ModifiedByAdminId = o.ModifiedByAdminId,
                    ModifiedDate = o.ModifiedDate ?? o.OrderDate,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.ProductName ?? "Unknown Product",
                        ProductImage = oi.ProductImage ?? "",
                        Quantity = oi.Quantity,
                        TotalPrice = oi.TotalPrice // Product price

                    }).ToList()
                }).ToList();

                return new ResponseDto<List<OrderDto>>(orderDtos, "Orders retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<OrderDto>>(null, ex.Message, 500);
            }
        }




        public async Task<ResponseDto<string>> DeleteProduct(int productId)
        {
            try
            {
                var product = await _appDbContext.Products.FindAsync(productId);

                if (product == null)
                {
                    return new ResponseDto<string>(null, "Product not found.", 404);
                }

                // Remove related data (Cart, Wishlist, Orders)
                _appDbContext.CartItems.RemoveRange(_appDbContext.CartItems.Where(ci => ci.ProductId == productId));
                _appDbContext.WishLists.RemoveRange(_appDbContext.WishLists.Where(wl => wl.ProductId == productId));
                //_appDbContext.OrderItems.RemoveRange(_appDbContext.OrderItems.Where(oi => oi.ProductId == productId));

                // Remove product from database (🔥 Hard Delete)
                _appDbContext.Products.Remove(product);

                await _appDbContext.SaveChangesAsync(); // Save changes

                return new ResponseDto<string>("Product deleted successfully.", "Success", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<string>(null, "Error deleting product: " + ex.Message, 500);
            }
        }



        //public async Task<ResponseDto<AddProductRes>> UpdateProductAsync(int productId, AddProductDto productData)
        //{
        //    try
        //    {
        //        var existingProduct = await _appDbContext.Products.FindAsync(productId);
        //        if (existingProduct == null)
        //        {
        //            return new ResponseDto<AddProductRes>(null, "Product not found", 404);
        //        }


        //        // ✅ Store old product name & image before updating
        //        var oldProductName = existingProduct.Name;
        //        var oldProductImage = existingProduct.Image;

        //        // Check if a new image is uploaded
        //        if (productData.Image != null && productData.Image.Length > 0)
        //        {
        //            var imageUrl = await _cloudinary.UploadImageAsync(productData.Image);
        //            if (string.IsNullOrEmpty(imageUrl))
        //            {
        //                return new ResponseDto<AddProductRes>(null, "Image upload failed", 500);
        //            }
        //            existingProduct.Image = imageUrl;
        //        }

        //        // Update other product properties
        //        existingProduct.Name = productData.Name;
        //        existingProduct.Description = productData.Description;
        //        existingProduct.RP = productData.RP;
        //        existingProduct.MRP = productData.MRP;
        //        existingProduct.Stock = productData.Stock;
        //        existingProduct.CategoryId = productData.CategoryId;
        //        existingProduct.Seller = productData.Seller;
        //        existingProduct.Ingredients = productData.Ingredients;

        //        _appDbContext.Products.Update(existingProduct);
        //        await _appDbContext.SaveChangesAsync();

        //        var resProduct = _mapper.Map<AddProductRes>(existingProduct);
        //        return new ResponseDto<AddProductRes>(resProduct, "Product updated successfully", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto<AddProductRes>(null, ex.Message, 500);
        //    }
        //}


        public async Task<ResponseDto<string>> UpdateOrderStatus(int orderId, OrderStatusEnum newStatus, int adminId)
        {
            try
            {
                var order = await _appDbContext.Orders.FindAsync(orderId);
                if (order == null)
                    return new ResponseDto<string>(null, "Order not found.", 404);

                if (order.OrderStatus == newStatus)
                    return new ResponseDto<string>(null, "Order is already in the requested status.", 400);

                order.OrderStatus = newStatus;
                order.ModifiedByAdminId = adminId;
                order.ModifiedDate = DateTime.UtcNow;

                await _appDbContext.SaveChangesAsync();

                return new ResponseDto<string>("Order status updated successfully.", "Success", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<string>(null, $"An unexpected error occurred: {ex.Message}", 500);
            }
        }
        private bool IsInvalidString(string value)
        {
            return string.IsNullOrWhiteSpace(value) || value == "string";
        }


        public async Task<ResponseDto<AddProductRes>> UpdateProductAsync(int productId, AddProductDto productData)
        {
            try
            {


                var existingProduct = await _appDbContext.Products.FindAsync(productId);
                if (existingProduct == null)
                {
                    return new ResponseDto<AddProductRes>(null, "Product not found", 404);
                }

                if (productData.Image != null && productData.Image.Length > 0)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(productData.Image);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        existingProduct.Image = imageUrl; // Update image only if a new one is uploaded
                    }
                }

                existingProduct.Name = IsInvalidString(productData.Name) ? existingProduct.Name : productData.Name;
                existingProduct.Description = IsInvalidString(productData.Description) ? existingProduct.Description : productData.Description;
                existingProduct.RP = productData.RP.Value > 0 ? productData.RP.Value : existingProduct.RP;
                existingProduct.MRP = productData.MRP.Value > 0 ? productData.MRP.Value : existingProduct.MRP;
                existingProduct.Stock = productData.Stock.Value > 0 ? productData.Stock.Value : existingProduct.Stock;
                existingProduct.CategoryId = productData.CategoryId > 0 ? productData.CategoryId : existingProduct.CategoryId;
                existingProduct.Seller = IsInvalidString(productData.Seller) ? existingProduct.Seller : productData.Seller;
                existingProduct.Ingredients = (productData.Ingredients == null || productData.Ingredients.Contains("string"))
                    ? existingProduct.Ingredients
                    : productData.Ingredients;

                _appDbContext.Products.Update(existingProduct);
                await _appDbContext.SaveChangesAsync();

                var resProduct = _mapper.Map<AddProductRes>(existingProduct); // ✅ Map updated product
                return new ResponseDto<AddProductRes>(resProduct, "Product updated successfully", 200);

            }
            catch (Exception ex)
            {
                return new ResponseDto<AddProductRes>(null, "Error updating product: " + ex.Message, 500);
            }
        }



    }
}