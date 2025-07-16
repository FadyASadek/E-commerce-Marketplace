using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Repository;
using System.Threading.Tasks;

[Authorize(Roles = "Seller")]
public class SellerDashboardController : Controller
{
    private readonly IOrderRepository _orderRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public SellerDashboardController(IOrderRepository orderRepo, UserManager<ApplicationUser> userManager)
    {
        _orderRepo = orderRepo;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var sellerId = _userManager.GetUserId(User);

        var viewModel = new SellerDashboardViewModel
        {
            TotalSales = await _orderRepo.GetTotalSalesBySellerIdAsync(sellerId),
            ItemsSoldCount = await _orderRepo.GetTotalItemsSoldCountBySellerIdAsync(sellerId),
            ActiveOrdersCount = await _orderRepo.GetActiveOrdersCountBySellerIdAsync(sellerId)
        };

        return View(viewModel);
    }
   
    public async Task<IActionResult> MySales()
    {
        var sellerId = _userManager.GetUserId(User);
        var orderDetails = await _orderRepo.GetOrderDetailsBySellerIdAsync(sellerId);
        return View(orderDetails);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessOrder(int orderDetailId)
    {
        var sellerId = _userManager.GetUserId(User);
        var orderDetail = await _orderRepo.GetOrderDetailByIdAsync(orderDetailId);

        if (orderDetail == null || orderDetail.SellerId != sellerId || orderDetail.Order.OrderStatus != "Pending")
        {
            return RedirectToAction(nameof(Index));
        }

        await _orderRepo.UpdateOrderStatusAsync(orderDetail.OrderId, "Processing");
        await _orderRepo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ShipOrder(int orderDetailId)
    {
        var sellerId = _userManager.GetUserId(User);
        var orderDetail = await _orderRepo.GetOrderDetailByIdAsync(orderDetailId);

        if (orderDetail == null || orderDetail.SellerId != sellerId || orderDetail.Order.OrderStatus != "Processing")
        {
            return RedirectToAction(nameof(Index));
        }

        await _orderRepo.UpdateOrderStatusAsync(orderDetail.OrderId, "Shipped");
        await _orderRepo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}