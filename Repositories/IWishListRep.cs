using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pet_s_Land.Datas;
using Pet_s_Land.DTOs;
using Pet_s_Land.Models.WhishlistModel;

namespace Pet_s_Land.Repositories
{
    public interface IWishListRep
    {
        public  Task<ResponseDto<object>> AddorRemove(int userId, int productId);
        public Task<ResponseDto<List<WishListResDto>>> GetWishList(int userId);
    }

    public class WishListRep : IWishListRep
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;



        public WishListRep(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<ResponseDto<object>> AddorRemove(int userId, int productId)
        {
            try
            {
                var productExists = await _appDbContext.Products.AnyAsync(p => p.Id == productId);
                if (!productExists)
                {
                    return new ResponseDto<object>(null, "Product does not exist.", 404, "Product not found.");
                }
                var isExist = await _appDbContext.WishLists.Include(x => x.Products).FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);
                if (isExist == null)
                {
                    WishListDto wishlistdto = new WishListDto
                    {
                        UserId = userId,
                        ProductId = productId,
                    };

                    var wishlist = _mapper.Map<WishList>(wishlistdto);
                    _appDbContext.WishLists.Add(wishlist);
                    await _appDbContext.SaveChangesAsync();
                    return new ResponseDto<object>(null, "Item added to wishlist successfully.", 200);
                }
                else
                {
                    _appDbContext.WishLists.Remove(isExist);
                    await _appDbContext.SaveChangesAsync();
                    return new ResponseDto<object>(null, "Item removed from wishlist successfully.", 200);
                
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>(null, "An error occurred.", 500, ex.Message);
            }

        }



        public async Task<ResponseDto<List<WishListResDto>>> GetWishList(int userId)
        {
            try
            {
                var wishlistItems = await _appDbContext.WishLists
                    .Where(w => w.UserId == userId && w.Products != null && !w.Products.IsDeleted) // Exclude deleted products
                    .Include(w => w.Products)
                    .Select(w => new WishListResDto
                    {
                        Id = w.Id,
                        ProductId = w.ProductId,
                        Name = w.Products.Name,
                        Price = w.Products.RP,
                        Image = w.Products.Image,
                        Category = w.Products.Category.CategoryName,
                        Description = w.Products.Description
                    })
                    .ToListAsync();

                if (!wishlistItems.Any())
                {
                    return new ResponseDto<List<WishListResDto>>(new List<WishListResDto>(), "No items in wishlist.", 404);
                }

                return new ResponseDto<List<WishListResDto>>(wishlistItems, "Wishlist retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<WishListResDto>>(null, "Error fetching wishlist", 500, ex.Message);
            }
        }





    }


}
