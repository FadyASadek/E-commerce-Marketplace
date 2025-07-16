using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly myContext _context;

        public ShoppingCartRepository(myContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCartItem?> GetCartItemAsync(string customerId, int productId)
        {
            return await _context.ShoppingCartItems
                .FirstOrDefaultAsync(item => item.CustomerId == customerId && item.ProductId == productId);
        }

        public async Task AddItemAsync(ShoppingCartItem item)
        {
            await _context.ShoppingCartItems.AddAsync(item);
        }

        public Task UpdateItemAsync(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Update(item);
            return Task.CompletedTask;
        }

        public async Task RemoveItemAsync(int itemId)
        {
            var item = await _context.ShoppingCartItems.FindAsync(itemId);
            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
            }
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string customerId)
        {
            return await _context.ShoppingCartItems
                .Where(item => item.CustomerId == customerId)
                .Include(item => item.Product)
                    .ThenInclude(p => p.User) 
                .ToListAsync();
        }

        public async Task ClearCartAsync(string customerId)
        {
            var cartItems = await _context.ShoppingCartItems
                                          .Where(item => item.CustomerId == customerId)
                                          .ToListAsync();

            if (cartItems.Any())
            {
                _context.ShoppingCartItems.RemoveRange(cartItems);
            }
        }
        public async Task<int> GetCartCountAsync(string customerId)
        {
            return await _context.ShoppingCartItems
                .Where(item => item.CustomerId == customerId)
                .SumAsync(item => item.Quantity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public Task RemoveItemsAsync(IEnumerable<ShoppingCartItem> items)
        {
            _context.ShoppingCartItems.RemoveRange(items);
            return Task.CompletedTask; 
        }
        public async Task<ShoppingCartItem?> GetCartItemByIdAsync(int itemId)
        {
            return await _context.ShoppingCartItems
                .Include(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == itemId);
        }
    }
}