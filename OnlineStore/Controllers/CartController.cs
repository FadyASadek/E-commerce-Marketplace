using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModels;
using System.Linq;
using System.Threading.Tasks;

public class AddToCartViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}


[Authorize]
public class CartController : Controller
{
    private readonly IShoppingCartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(
     IShoppingCartRepository cartRepo,
     IProductRepository productRepo,
     UserManager<ApplicationUser> userManager)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo; 
        _userManager = userManager;
    }

   
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var allCartItems = await _cartRepo.GetCartItemsAsync(userId);

        var validCartItems = allCartItems.Where(item => item.Product != null).ToList();

        var orphanedItems = allCartItems.Where(item => item.Product == null).ToList();

        if (orphanedItems.Any())
        {
            await _cartRepo.RemoveItemsAsync(orphanedItems);
            await _cartRepo.SaveChangesAsync();
        }

        var cartViewModel = new CartViewModel
        {
            CartItems = validCartItems,
            GrandTotal = validCartItems.Sum(item => (decimal)item.Product.Price * item.Quantity)
        };

        foreach (var item in validCartItems)
        {
            if (item.Quantity > item.Product.Quantity)
            {
                cartViewModel.StockErrors[item.Id] = $"Only {item.Product.Quantity} items available.";
            }
        }

        return View(cartViewModel);
    }

    
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartViewModel model)
    {
        if (model == null || model.ProductId <= 0 || model.Quantity <= 0)
        {
            return BadRequest(new { success = false, message = "Invalid data." });
        }

        var userId = _userManager.GetUserId(User);
        
        var product = _productRepo.GetProductById(model.ProductId); 
        if (product == null) 
        {
            return NotFound(new { success = false, message = "Product not found." });
        }

        var cartItem = await _cartRepo.GetCartItemAsync(userId, model.ProductId);
        int quantityInCart = cartItem?.Quantity ?? 0;

        if ((quantityInCart + model.Quantity) > product.Quantity)
        {
            return BadRequest(new { success = false, message = $"Sorry, only {product.Quantity} items are available." });
        }

        if (cartItem == null)
        {
            cartItem = new ShoppingCartItem
            {
                CustomerId = userId,
                ProductId = model.ProductId,
                Quantity = model.Quantity
            };
            await _cartRepo.AddItemAsync(cartItem);
        }
        else
        {
            cartItem.Quantity += model.Quantity;
            await _cartRepo.UpdateItemAsync(cartItem);
        }

        await _cartRepo.SaveChangesAsync();
        var cartCount = await _cartRepo.GetCartCountAsync(userId);
        return Ok(new { success = true, message = "Product added to cart!", cartCount = cartCount });
    }
    
    
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int itemId, int newQuantity)
    {
        var cartItem = await _cartRepo.GetCartItemByIdAsync(itemId);
        if (cartItem == null) 
        {
            return NotFound(new { success = false, message = "Item not found in cart." });
        }

        if (newQuantity > cartItem.Product.Quantity)
        {
            return BadRequest(new { success = false, message = $"Only {cartItem.Product.Quantity} items available." });
        }

        if (newQuantity < 1)
        {
            await _cartRepo.RemoveItemAsync(itemId);
        }
        else
        {
            cartItem.Quantity = newQuantity;
            await _cartRepo.UpdateItemAsync(cartItem);
        }
        
        await _cartRepo.SaveChangesAsync();
        return await GetCartSummaryForJson();
    }

    
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int itemId)
    {
        var itemToRemove = await _cartRepo.GetCartItemByIdAsync(itemId);
        if(itemToRemove == null)
        {
            return NotFound(new { success = false, message = "Item not found to remove." });
        }

        await _cartRepo.RemoveItemAsync(itemId);
        await _cartRepo.SaveChangesAsync();
        return await GetCartSummaryForJson();
    }

   
    private async Task<IActionResult> GetCartSummaryForJson()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _cartRepo.GetCartItemsAsync(userId);
        
        var grandTotal = cartItems.Sum(i => (decimal)i.Product.Price * i.Quantity);
        var cartCount = cartItems.Sum(i => i.Quantity);
        bool hasStockErrors = cartItems.Any(i => i.Quantity > i.Product.Quantity);

        return Ok(new {
            success = true,
            grandTotal = grandTotal.ToString("F2"),
            cartCount = cartCount,
            hasStockErrors = hasStockErrors
        });
    }
}