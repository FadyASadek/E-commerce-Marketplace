using OnlineStore.Models;
using OnlineStore.ViewModel.Category;
using OnlineStore.ViewModel.Home;

namespace OnlineStore.Repository
{
    public interface ICategoryRepository
    {
       IEnumerable<MainCategory> GetAll();
        MainCategory GetCategory(int id);
        void AddCategory(MainCategory category);
        void EditCategory(MainCategory category);
        void DeleteCategory(int id);
        IEnumerable<MainCategory> GetTopCategory(int count  = 4);
        Task<IEnumerable<CategoryWithProductCountViewModel>> GetAllCategoryWithProductCount();


    }
}
