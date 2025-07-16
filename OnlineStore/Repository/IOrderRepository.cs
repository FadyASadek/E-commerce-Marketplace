using OnlineStore.Models;
using System.Threading.Tasks;

namespace OnlineStore.Repository
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsBySellerIdAsync(string sellerId);
        Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<IEnumerable<Order>> GetAllOrdersAsync(string? statusFilter = null, string? searchQuery = null);
        Task<decimal> GetTotalSalesBySellerIdAsync(string sellerId);
        Task<int> GetTotalItemsSoldCountBySellerIdAsync(string sellerId);
        Task<int> GetActiveOrdersCountBySellerIdAsync(string sellerId);
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<int> SaveChangesAsync(); 
    }
}