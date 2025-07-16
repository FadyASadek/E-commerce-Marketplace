using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using System.Threading.Tasks;

namespace OnlineStore.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly myContext _context; 

        public OrderRepository(myContext context)
        {
            _context = context;
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .ThenInclude(od => od.Product)
                                 .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                                 .Where(o => o.CustomerId == userId)
                                 .Include(o => o.OrderDetails)
                                 .ThenInclude(od => od.Product) 
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();
        }
        public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
                return null;

            return await _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .ThenInclude(od => od.Product)
                                 .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsBySellerIdAsync(string sellerId)
        {
            return await _context.OrderDetails
                                 .Where(od => od.SellerId == sellerId)
                                 .Include(od => od.Order) 
                                    .ThenInclude(o => o.Customer) 
                                 .Include(od => od.Product) 
                                 .OrderByDescending(od => od.Order.OrderDate)
                                 .ToListAsync();
        }
        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails
                                 .Include(od => od.Order)
                                 .FirstOrDefaultAsync(od => od.Id == orderDetailId);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false; 
            }
            order.OrderStatus = newStatus;
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? statusFilter = null, string? searchQuery = null)
        {
            var query = _context.Orders.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.OrderStatus == statusFilter);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var lowerCaseQuery = searchQuery.ToLower();
                query = query.Where(o => o.OrderNumber.ToLower().Contains(lowerCaseQuery) || o.Customer.UserName.ToLower().Contains(lowerCaseQuery));
            }

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }
        public async Task<decimal> GetTotalSalesBySellerIdAsync(string sellerId)
        {
            var completedStatuses = new[] { "Shipped", "Delivered" };
            return await _context.OrderDetails
                .Where(od => od.SellerId == sellerId && completedStatuses.Contains(od.Order.OrderStatus))
                .SumAsync(od => od.Quantity * od.Price);
        }

        public async Task<int> GetTotalItemsSoldCountBySellerIdAsync(string sellerId)
        {
            var completedStatuses = new[] { "Shipped", "Delivered" };
            return await _context.OrderDetails
                .Where(od => od.SellerId == sellerId && completedStatuses.Contains(od.Order.OrderStatus))
                .SumAsync(od => od.Quantity);
        }

        public async Task<int> GetActiveOrdersCountBySellerIdAsync(string sellerId)
        {
            var activeStatuses = new[] { "Pending", "Processing" };
            return await _context.OrderDetails
                .Where(od => od.SellerId == sellerId && activeStatuses.Contains(od.Order.OrderStatus))
                .Select(od => od.OrderId) 
                .Distinct()
                .CountAsync();
        }
        public async Task<decimal> GetTotalRevenueAsync()
        {
            var completedStatuses = new[] { "Shipped", "Delivered" };
            return await _context.OrderDetails
                .Where(od => completedStatuses.Contains(od.Order.OrderStatus))
                .SumAsync(od => od.Quantity * od.Price);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}