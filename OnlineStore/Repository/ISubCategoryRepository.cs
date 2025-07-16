using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public interface ISubCategoryRepository
    {
         IEnumerable<MSubCategory> GetAll();
         MSubCategory GetSubCategory(int id);
         void AddSubCategory(MSubCategory subCategory);
         void UpdateSubCategory(int id ,MSubCategory subCategory);
         void DeleteSubCategory(int id);
        IEnumerable<MSubCategory> GetSubCategoriesByCategortId(int id);
    }
}
