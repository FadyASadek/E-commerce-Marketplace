using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public class SubCategoryRepostory : ISubCategoryRepository
    {
        private readonly myContext context;

        public SubCategoryRepostory(myContext context)
        {
            this.context = context;
        }
        public void AddSubCategory(MSubCategory subCategory)
        {
            try
            {
                context.subCategories.Add(subCategory);
                context.SaveChanges();
            }
            catch (Exception ex) {
                throw new Exception("An error occurred while adding the SubCategry.", ex);
            }
        }

        public void DeleteSubCategory(int id)
        {
            context.subCategories.Remove(GetSubCategory(id));
            context.SaveChanges();
        }

        public MSubCategory GetSubCategory(int id)
        {
            
            return context.subCategories.Include(c => c.category).FirstOrDefault(c => c.Id == id)!;
            
           
             
        }

        public IEnumerable<MSubCategory> GetAll()
        {
           return context.subCategories.Include(c=>c.category).ToList();
        }

        public void UpdateSubCategory(int id, MSubCategory subCategory)
        {
            MSubCategory cat = GetSubCategory(id);
            cat.Name = subCategory.Name;
            cat.Description = subCategory.Description;
            cat.Image = subCategory.Image;
            cat.categoryId = subCategory.categoryId;
            context.SaveChanges();
        }

        public IEnumerable<MSubCategory> GetSubCategoriesByCategortId(int id)
        {
            return context.subCategories.Where(c => c.categoryId == id).ToList();
        }
    }
}
