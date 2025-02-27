using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.CartModels;

namespace Pet_s_Land.Repositories
{
    public interface ICartRepo
    {
        Task<ResponseDto<CartResDto>> GetCartItems(int userId);
        Task<ResponseDto<object>> AddToCart(int userId, int productId);
        Task<ResponseDto<object>> RemoveFromCart(int userId, int productId);
        Task<ResponseDto<object>> IncreaseQty(int userId, int productId);
        Task<ResponseDto<object>> DecreaseQty(int userId, int productId);
        Task<ResponseDto<object>> RemoveAllItems(int userId);
    }
    public class CartRepo : ICartRepo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;


        public CartRepo(AppDbContext appDbContext, IMapper mapper)
        {

            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseDto<object>> AddToCart(int userId, int productId)
        {
            try
            {
                var user = await _appDbContext.Users.Include(c => c.Cart)
                                    .ThenInclude(c => c.CartItems)
                                    .ThenInclude(c => c.Product)
                                    .FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    return new ResponseDto<object>(null, "User not found", 404);
                }
                var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    return new ResponseDto<object>(null, $"Product with {productId} is not found", 404);
                }
                if (product?.Stock == 0)
                {

                    return new ResponseDto<object>(null, "Out of stock", 404);
                }
                if (product != null && user != null)
                {
                    if (user.Cart == null)
                    {
                        user.Cart = new Cart
                        {
                            UserId = userId,
                            CartItems = new List<CartItem>()
                        };
                        _appDbContext.Carts.Add(user.Cart);
                        await _appDbContext.SaveChangesAsync();

                    }
                }
                var check = user.Cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
                if (check != null)
                {
                    if (check.Quantity >= product.Stock)
                    {
                        return new ResponseDto<object>(null, "Out of stock", 400);
                    }
                    if (check.Quantity < product.Stock)
                    {
                        check.Quantity++;
                        await _appDbContext.SaveChangesAsync();
                        return new ResponseDto<object>(null, "Product Quantity increased successfully", 200);
                    }
                }
                var item = new CartItem
                {
                    CartId = user.Cart.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                user.Cart.CartItems.Add(item);
                await _appDbContext.SaveChangesAsync();
                return new ResponseDto<object>(null, "Product added successfully", 200);


            }
            catch (Exception ex)
            {
                return new ResponseDto<object>(null, "Internal Server Error Occured", 500);
            }


        }


        public async Task<ResponseDto<CartResDto>> GetCartItems(int userId)
        {
            try
            {
                if (userId == 0)
                {
                    return new ResponseDto<CartResDto>(null, "User Id is null", 404);
                }

                var cart = await _appDbContext.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (cart != null)
                {
                    var cartitem = cart.CartItems.Select(x => new CartViewDto
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        Price = x.Product.Price,
                        Quantity = x.Quantity,
                        TotalAmount = x.Quantity * x.Product.Price,
                        Image = x.Product.Image
                    }).ToList();

                    var TotalItems = cartitem.Count;
                    var TotalPrice = cartitem.Sum(i => i.TotalAmount);

                    var result = new CartResDto
                    {
                        TotalItem = TotalItems,
                        TotalPrice = TotalPrice,
                        cartItemsperUser = cartitem
                    };

                    return new ResponseDto<CartResDto>(result, "Cart retrived successfully", 200);
                }
                var Noresult = new CartResDto
                {
                    TotalItem = 0,
                    TotalPrice = 0,
                    cartItemsperUser = []

                };

                return new ResponseDto<CartResDto>(Noresult, "No itmes in cart.", 404);
            }
            catch (Exception ex)
            {
                return new ResponseDto<CartResDto>(null, "Internal Server Error Occurred: " + ex.Message, 500);
            }
        }


        public async Task<ResponseDto<object>> RemoveFromCart(int userId, int productId)
        {
            try
            {
                var user = await _appDbContext.Users.Include(u => u.Cart)
                    .ThenInclude(u => u.CartItems)
                    .ThenInclude(u => u.Product)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);

                if (user == null)
                {
                    return new ResponseDto<object>(null, "User not found", 404);
                }
                if (user.Cart == null)
                {
                    return new ResponseDto<object>(null, "Cart not found for the user", 404);
                }

                var item = user.Cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

                if (item == null)
                {
                    return new ResponseDto<object>(null, $"Product with ID {productId} not found in cart", 404);
                }

                user.Cart.CartItems.Remove(item);
                await _appDbContext.SaveChangesAsync();

                return new ResponseDto<object>(null, "Product removed successfully", 200);

            }
            catch (Exception ex)
            {
                return new ResponseDto<object>(null, "Internal Server Error Occurred: " + ex.Message, 500);

            }

        }

        public async Task<ResponseDto<object>> IncreaseQty(int userId, int productId)
        {
            try
            {
                var user = await _appDbContext.Users.Include(u => u.Cart)
                    .ThenInclude(u => u.CartItems)
                    .ThenInclude(u => u.Product).FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new ResponseDto<object>(null, "User not found.", 404);
                }

                if (user.Cart == null)
                {
                    return new ResponseDto<object>(null, "Cart not found.", 404);
                }
                var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    return new ResponseDto<object>(null, "Product not found.", 404);

                }

                var item = user.Cart.CartItems.FirstOrDefault(u => u.ProductId == productId);

                if (item == null)
                {
                    return new ResponseDto<object>(null, $"Product with ID {productId} not found", 404);
                }

                if (item.Quantity >= 10)
                {
                    return new ResponseDto<object>(null, "Maximum quantity limit (10) reached.", 400);
                }
                if (product.Stock <= item.Quantity)
                {
                    return new ResponseDto<object>(null, "Cannot increase quantity beyond available stock.", 400);
                }

                item.Quantity++;
                await _appDbContext.SaveChangesAsync();
                return new ResponseDto<object>(null, "Quatity incresed successfully", 200);


            }
            catch (Exception ex)
            {
                return new ResponseDto<object>(null, "Internal Server Error Occured", 500);
            }

        }

        public async Task<ResponseDto<object>> DecreaseQty(int userId, int productId)
        {
            try
            {
                // Fetch user with cart and product details
                var user = await _appDbContext.Users
                    .Include(u => u.Cart)
                    .ThenInclude(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new ResponseDto<object>(null, "User not found.", 404);
                }

                if (user.Cart == null)
                {
                    return new ResponseDto<object>(null, "Cart not found.", 404);
                }

                var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    return new ResponseDto<object>(null, "Product not found.", 404);
                }

                var item = user.Cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (item == null)
                {
                    return new ResponseDto<object>(null, $"Product with ID {productId} not found in cart.", 404);
                }

                if (item.Quantity == 1)
                {
                    user.Cart.CartItems.Remove(item);
                    await _appDbContext.SaveChangesAsync();
                    return new ResponseDto<object>(null, "Product removed from cart.", 200);
                }

                item.Quantity--;
                await _appDbContext.SaveChangesAsync();

                return new ResponseDto<object>(null, "Quantity decreased successfully.", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>(null, "Internal Server Error Occurred: " + ex.Message, 500);
            }
        }

        public async Task<ResponseDto<object>> RemoveAllItems(int userId)
        {
            try
            {
                var user = _appDbContext.Users.Include(u => u.Cart)
                    .ThenInclude(c => c.CartItems)
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return new ResponseDto<object>(null, "User not found.", 404);
                }
                if (user.Cart == null || user.Cart.CartItems.Count == 0)
                {
                    return new ResponseDto<object>(null, "Cart is already empty.", 200);

                }
                user.Cart.CartItems.Clear();
                await _appDbContext.SaveChangesAsync();
                return new ResponseDto<object>(null, "All items removed from cart successfully.", 200);
            }
            catch (Exception ex) 
            {
                return new ResponseDto<object>(null, "Internal Server Error Occurred: " + ex.Message, 500);

            }
        }




    }

}




