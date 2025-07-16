using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.Admin;

namespace OnlineStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            UserManager<ApplicationUser> userManager)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalRevenue = await _orderRepo.GetTotalRevenueAsync(),
                TotalOrders = await _orderRepo.GetTotalOrdersCountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalProducts = _productRepo.GetProductCount() 
            };

            return View(viewModel);
        }

        
    }
}
