using OnlineStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Repository
{
    public interface IShoppingCartRepository
    {
       
        Task<ShoppingCartItem?> GetCartItemAsync(string customerId, int productId);      
        Task AddItemAsync(ShoppingCartItem item);
        Task UpdateItemAsync(ShoppingCartItem item);
        Task RemoveItemAsync(int itemId);
        Task<IEnumerable<ShoppingCartItem>> GetCartItemsAsync(string customerId);
        Task ClearCartAsync(string customerId);
        Task<ShoppingCartItem?> GetCartItemByIdAsync(int itemId);
        Task<int> GetCartCountAsync(string customerId);
        Task RemoveItemsAsync(IEnumerable<ShoppingCartItem> items);
        Task<int> SaveChangesAsync();
    }
}