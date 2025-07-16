using OnlineStore.Models;

namespace OnlineStore.ViewModel.Home
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}
