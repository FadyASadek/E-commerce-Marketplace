using OnlineStore.Models;

namespace OnlineStore.ViewModel.Home
{
    public class HomeCategoryAndProductVM
    {
        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<MainCategory> mainCategories { get; set; }
    }
}
