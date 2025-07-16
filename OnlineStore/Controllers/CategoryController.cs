using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.Category;
using System.Linq;

namespace OnlineStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMemoryCache cache;

        public CategoryController(ICategoryRepository categoryRepository,IMemoryCache cache)
        {
            this.categoryRepository = categoryRepository;
            this.cache = cache;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<MainCategory> categories = categoryRepository.GetAll();

            return View(categories);

        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CategoryVM category)
        {
            if (!ModelState.IsValid)
            {
                return View("AddCategory", category); 
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            string primaryImagePath = null;

            if (category.primaryImage != null && category.primaryImage.Length > 0)
            {
                var fileExtension = Path.GetExtension(category.primaryImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("PrimaryImage", "Image format is not supported. Allowed formats: JPG, PNG, GIF");
                    return View("AddCategory", category);
                }

                try
                {
                    var uploadsFolder = Path.Combine("wwwroot/images");

                    Directory.CreateDirectory(uploadsFolder);

                    var primaryFileName = Guid.NewGuid() + fileExtension;

                    primaryImagePath = Path.Combine(uploadsFolder, primaryFileName);

                    using (var stream = new FileStream(primaryImagePath, FileMode.Create))
                    {
                        await category.primaryImage.CopyToAsync(stream);
                    }

                    primaryImagePath = "/images/" + primaryFileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("PrimaryImage", $"An error occurred while uploading the image : {ex.Message}");
                    return View("AddCategory", category);
                }
            }
                
            var cat = new MainCategory
            {
                Name = category.name,
                Image = primaryImagePath, 
                Description = category.description
            };

            try
            {
                categoryRepository.AddCategory(cat);
                cache.Remove("CategoriesCache");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving data: {ex.Message}");
                return View("AddCategory", category);
            }

            return RedirectToAction("Index", "Category");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditCategory(int id)
        {
            var category =  categoryRepository.GetCategory(id);
           

            var categoryVM = new EditCategoryVm
            {
                id = category.Id,
                name = category.Name,
                description = category.Description,
                CurrentImage = category.Image 
            };

            return View(categoryVM);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, EditCategoryVm category)
        {
            if (id != category.id)
            {
                ModelState.AddModelError("", "The category id invalid");
                return View(category);
            }
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            var cat = categoryRepository.GetCategory(id);

            string primaryImagePath = cat.Image;
            category.CurrentImage = primaryImagePath;

            if (category.PrimaryImage != null && category.PrimaryImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(category.PrimaryImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("Image", "Image format is not supported. Allowed formats: JPG, PNG, GIF");
                    return View(category);
                }

                var uploadsFolder = Path.Combine("wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);
                var primaryFileName = Guid.NewGuid() + fileExtension;
                primaryImagePath = Path.Combine(uploadsFolder, primaryFileName);

               
                using (var stream = new FileStream(primaryImagePath, FileMode.Create))
                {
                    await category.PrimaryImage.CopyToAsync(stream);
                }

                primaryImagePath = "/images/" + primaryFileName;

                if (!string.IsNullOrEmpty(category.CurrentImage))
                {
                    var oldImagePath = Path.Combine("wwwroot", category.CurrentImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }
            
            var updatedCategory = new MainCategory
            {
                Id = id,
                Name = category.name,
                Description = category.description,
                Image = primaryImagePath
            };

            try
            {
                categoryRepository.EditCategory(updatedCategory);
                cache.Remove("CategoriesCache");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while updating the category: {ex.Message}");
                return View(category);
            }

            return RedirectToAction("Index", "Category");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var cat = categoryRepository.GetCategory(id);
            if (!string.IsNullOrEmpty(cat.Image))
            {
                var oldImagePath = Path.Combine("wwwroot", cat.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            categoryRepository.DeleteCategory(id);
            cache.Remove("CategoriesCache");
            return RedirectToAction(nameof(Index));
        } 
    }
}
