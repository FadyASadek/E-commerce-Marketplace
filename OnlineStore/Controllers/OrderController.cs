using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModels;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class OrderController : Controller
{
    private readonly IShoppingCartRepository _cartRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly myContext _context; 

    public OrderController(
        IShoppingCartRepository cartRepo,
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        UserManager<ApplicationUser> userManager,
        myContext context) 
    {
        _cartRepo = cartRepo;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _userManager = userManager;
        _context = context;
    }
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _cartRepo.GetCartItemsAsync(userId);

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        foreach (var item in cartItems)
        {
            if (item.Quantity > item.Product.Quantity)
            {
                TempData["ErrorMessage"] = $"The quantity for product '{item.Product.Name}' is no longer available. Please update your cart.";
                return RedirectToAction("Index", "Cart");
            }
        }

        var checkoutViewModel = new CheckoutViewModel
        {
            CartItems = cartItems,
            GrandTotal = cartItems.Sum(item => (decimal)item.Product.Price * item.Quantity)
        };

        return View(checkoutViewModel);
    }
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel viewModel)
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _cartRepo.GetCartItemsAsync(userId);

        if (!ModelState.IsValid)
        {
            viewModel.CartItems = cartItems;
            viewModel.GrandTotal = cartItems.Sum(item => (decimal)item.Product.Price * item.Quantity);
            return View("Checkout", viewModel);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in cartItems)
            {
                var product = _productRepo.GetProductById(item.ProductId); 
                if (item.Quantity > product.Quantity)
                {
                    TempData["ErrorMessage"] = $"The quantity for '{product.Name}' is no longer available.";
                    await transaction.RollbackAsync(); 
                    return RedirectToAction("Index", "Cart");
                }
            }

            var order = new Order
            {
                CustomerId = userId,
                OrderNumber = $"ORD-{DateTime.Now.ToString("yyyyMMdd")}-{Path.GetRandomFileName().ToUpper().Substring(0, 6)}",

                OrderDate = DateTime.Now,
                OrderStatus = "Pending", 
                PaymentMethod = "CashOnDelivery",
                RecipientName = viewModel.RecipientName,
                RecipientPhone = viewModel.RecipientPhone,
                ShippingAddress = viewModel.ShippingAddress,
                City = viewModel.City,
                OrderTotal = cartItems.Sum(item => (decimal)item.Product.Price * item.Quantity)
            };

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    SellerId = item.Product.UserId, 
                    Quantity = item.Quantity,
                    Price = (decimal)item.Product.Price 
                };
                order.OrderDetails.Add(orderDetail);

                var productInStock = _productRepo.GetProductById(item.ProductId);
                productInStock.Quantity -= item.Quantity;
            }

            await _orderRepo.CreateOrderAsync(order);
            await _context.SaveChangesAsync(); 

            await _cartRepo.ClearCartAsync(userId);
            await _cartRepo.SaveChangesAsync();

            await transaction.CommitAsync();

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "An unexpected error occurred while placing your order. Please try again.";
            return RedirectToAction("Index", "Cart");
        }
    }
    [Authorize]
    public async Task<IActionResult> Confirmation(int id)
    {
        var order = await _orderRepo.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound(); 
        }
        return View(order);
    }
    [Authorize]
    public async Task<IActionResult> MyOrders()
    {
        var userId = _userManager.GetUserId(User);
        var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
        return View(orders);
    }
    [Authorize]
    public async Task<IActionResult> OrderDetails(string id) 
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var order = await _orderRepo.GetOrderByOrderNumberAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        if (order.CustomerId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Seller"))
        {
            return Forbid();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _orderRepo.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        if (order.CustomerId != currentUserId)
        {
            return Forbid();
        }

        if (order.OrderStatus != "Pending" && order.OrderStatus != "Processing")
        {
            TempData["ErrorMessage"] = "This order cannot be cancelled as it has already been shipped.";
            return RedirectToAction(nameof(MyOrders));
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = _productRepo.GetProductById(detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity += detail.Quantity;
                    }
                }

                order.OrderStatus = "Cancelled";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Order #{order.OrderNumber} has been successfully cancelled.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "An error occurred while cancelling the order.";
            }
        }

        return RedirectToAction(nameof(MyOrders));
    }

    [Authorize(Roles = "Admin")] 
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AllOrders(string statusFilter, string searchQuery)
    {
        ViewData["Title"] = "All System Orders";

        var allOrders = await _orderRepo.GetAllOrdersAsync(statusFilter, searchQuery);

        ViewData["CurrentStatusFilter"] = statusFilter;
        ViewData["CurrentSearch"] = searchQuery;

        return View(allOrders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
    {
        if (string.IsNullOrEmpty(newStatus))
        {
            TempData["ErrorMessage"] = "Please select a valid status.";
            return RedirectToAction(nameof(AllOrders));
        }

        var success = await _orderRepo.UpdateOrderStatusAsync(orderId, newStatus);
        if (success)
        {
            await _orderRepo.SaveChangesAsync(); 
            TempData["SuccessMessage"] = $"Order #{orderId} status has been updated to '{newStatus}'.";
        }
        else
        {
            TempData["ErrorMessage"] = "Order not found.";
        }

        return RedirectToAction(nameof(AllOrders));
    }
}