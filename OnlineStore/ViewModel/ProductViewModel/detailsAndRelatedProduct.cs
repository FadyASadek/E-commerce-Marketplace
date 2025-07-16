
using OnlineStore.Models;

namespace OnlineStore.ViewModel.ProductViewModel
{
    public class detailsAndRelatedProduct
    {
        public Product product { get; set; }
        public IEnumerable<Product> ListOfProdect { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public IEnumerable<Commment> Comments { get; set; }

    }
}
