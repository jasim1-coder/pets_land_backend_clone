using AutoMapper;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
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
                var userdto = _mapper.Map<UserViewDTO>(user);
                return new ResponseDto<UserViewDTO>(userdto, "Users retrieved successfully", 200);



            }
            catch (Exception ex)
            {
                return new ResponseDto<UserViewDTO>(null, "Internal Server Error Occured", 500);

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
                    CustomerName = order.User.Name,
                    PhoneNumber = order.Address.PhoneNumber,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.ProductName,  //  Use stored product name from OrderItems
                        ProductImage = oi.ProductImage, //  Use stored product image from OrderItems
                        Quantity = oi.Quantity
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


        public async Task<ResponseDto<AddProductRes>> UpdateProductAsync(int productId, AddProductDto productData)
        {
            try
            {
                var existingProduct = await _appDbContext.Products.FindAsync(productId);
                if (existingProduct == null)
                {
                    return new ResponseDto<AddProductRes>(null, "Product not found", 404);
                }

                // ✅ Store old product image (for reference, but do not update past orders)
                var oldProductImage = existingProduct.Image;

                if (productData.Image != null && productData.Image.Length > 0)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(productData.Image);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return new ResponseDto<AddProductRes>(null, "Image upload failed", 500);
                    }
                    existingProduct.Image = imageUrl;
                }

                existingProduct.Name = productData.Name; // ✅ Update the product itself
                existingProduct.Description = productData.Description;
                existingProduct.RP = productData.RP;
                existingProduct.MRP = productData.MRP;
                existingProduct.Stock = productData.Stock;
                existingProduct.CategoryId = productData.CategoryId;
                existingProduct.Seller = productData.Seller;
                existingProduct.Ingredients = productData.Ingredients;

                _appDbContext.Products.Update(existingProduct);
                await _appDbContext.SaveChangesAsync();

                var resProduct = _mapper.Map<AddProductRes>(existingProduct);
                return new ResponseDto<AddProductRes>(resProduct, "Product updated successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<AddProductRes>(null, "Error updating product: " + ex.Message, 500);
            }
        }


    }
}