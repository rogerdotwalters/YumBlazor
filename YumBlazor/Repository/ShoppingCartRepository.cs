using Microsoft.EntityFrameworkCore;
using YumBlazor.Data;
using YumBlazor.Repository.IRepository;

namespace YumBlazor.Repository {
    public class ShoppingCartRepository : IShoppingCartRepository {

        private readonly ApplicationDbContext _dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext) {

            _dbContext = dbContext;
        }

        public async Task<bool> ClearCartAsync(string? userId) {

            var cartItems = await _dbContext.ShoppingCart.Where(q => q.UserId == userId).ToListAsync();
            _dbContext.ShoppingCart.RemoveRange(cartItems);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync(string? userId) {

            return await _dbContext.ShoppingCart.Where(q => q.UserId == userId).Include(q => q.Product).ToListAsync();
        }

        public async Task<bool> UpdateCartAsync(string userId, int productId, int updateBy) {

            if (string.IsNullOrEmpty(userId)) {

                return false;
            }
            var cart = await _dbContext.ShoppingCart.FirstOrDefaultAsync(q => q.UserId == userId && q.ProductId == productId);
            if (cart == null) {

                cart = new ShoppingCart {

                    UserId = userId,
                    ProductId = productId,
                    Count = updateBy
                };
                await _dbContext.ShoppingCart.AddAsync(cart);
            } else {

                cart.Count += updateBy;
                if (cart.Count <= 0) {

                    _dbContext.ShoppingCart.Remove(cart);
                }
            }
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
