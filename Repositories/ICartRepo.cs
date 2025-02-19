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
        //    Task<ResponseDto<object>> RemoveFromCart(int userId, int productId);
        //    Task<ResponseDto<object>> IncreaseQty(int userId, int productId);
        //    Task<ResponseDto<object>> DecreaseQty(int userId, int productId);
        //    Task<ResponseDto<object>> RemoveAllItems(int userId);
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

                        return new ResponseDto<CartResDto>(result, "", 200);
                    }

                    return new ResponseDto<CartResDto>(null, "Cart not found", 404);
                }
                catch (Exception ex)
                {
                    return new ResponseDto<CartResDto>(null, "Internal Server Error Occurred: " + ex.Message, 500);
                }
            }


     }

 }
 