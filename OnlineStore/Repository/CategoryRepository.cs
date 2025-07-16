using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.ViewModel.Category;
using OnlineStore.ViewModel.Home;

namespace OnlineStore.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly myContext context;

        public CategoryRepository(myContext context)
        {
            this.context = context;
        }
        public IEnumerable<MainCategory> GetAll()
        {
            return context.categories.AsNoTracking().ToList();
        }
        public void AddCategory(MainCategory category)
        {
            try
            {
                context.categories.Add(category);
                context.SaveChanges();
            }
            catch (Exception ex) {
                throw new Exception("An error occurred while adding the category.", ex);
            }
        }

        public void DeleteCategory(int id)
        {
            context.categories.Remove(GetCategory(id));
            context.SaveChanges();
        }

     

       
        public MainCategory GetCategory(int id)
        {
           return  context.categories.FirstOrDefault(c => c.Id == id);
        }

      
        public void EditCategory(MainCategory category)
        {
            var cate =GetCategory(category.Id);
            if (cate != null)
            {
                cate.Name = category.Name;
                cate.Description = category.Description;
                cate.Image = category.Image;
                context.SaveChanges();
            }
            
        }

        public IEnumerable<MainCategory> GetTopCategory(int count = 4)
        {
            return context.categories.Take(count).AsNoTracking().ToList();
        }
        public async Task<IEnumerable<CategoryWithProductCountViewModel>> GetAllCategoryWithProductCount()
        {
            var category = await context.categories.Select(c => new CategoryWithProductCountViewModel
            {
                id=c.Id,
              Name = c.Name,
              Description = c.Description,
              Image = c.Image,
              ProductCount = c.subCategories.SelectMany(s=> s.products).Count()
            }).ToListAsync();
            return category;
        }
    }
}
