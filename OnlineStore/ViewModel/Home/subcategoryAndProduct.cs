using OnlineStore.Models;

namespace OnlineStore.ViewModel.Home
{
    public class subcategoryAndProduct
    {
        public IEnumerable<Product> products { get; set; }
        public IEnumerable<MSubCategory> subCategories { get; set; }
    }
}
