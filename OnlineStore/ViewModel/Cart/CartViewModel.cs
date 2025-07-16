using OnlineStore.Models;
using System.Collections.Generic;

namespace OnlineStore.ViewModels
{ 
    public class CartViewModel
    {
        public IEnumerable<ShoppingCartItem> CartItems { get; set; }
        public decimal GrandTotal { get; set; }

        public Dictionary<int, string> StockErrors { get; set; } = new Dictionary<int, string>();
        public bool HasStockErrors => StockErrors.Any(); 
    }
}