using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.Category;
using OnlineStore.ViewModel.SubCategory;
using System.Linq;

namespace OnlineStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryRepository subCategoryRepository;
        private readonly ICategoryRepository categoryRepository;

        public SubCategoryController(ISubCategoryRepository subCategoryRepository,ICategoryRepository categoryRepository)
        {
            this.subCategoryRepository = subCategoryRepository;
            this.categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            IEnumerable<MSubCategory> subCategories = subCategoryRepository.GetAll();
            return View(subCategories);
        }
        [HttpGet]
        public IActionResult AddSubCategory()
        {
           var categories = categoryRepository.GetAll();
            ViewBag.category = categories;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubCategory(AddSubCategoryVM SubCategory)
        {
            if (!ModelState.IsValid)
            {
                var categories = categoryRepository.GetAll();
                ViewBag.category = categories;
                return View("AddSubCategory", SubCategory); 
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            string primaryImagePath = null;

            if (SubCategory.Image != null && SubCategory.Image.Length > 0)
            {
                var fileExtension = Path.GetExtension(SubCategory.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    var categories = categoryRepository.GetAll();
                    ViewBag.category = categories;
                    ModelState.AddModelError("PrimaryImage", "Image format is not supported. Allowed formats: JPG, PNG, GIF");
                    return View("AddSubCategory", SubCategory);
                }

                try
                {
                    var uploadsFolder = Path.Combine("wwwroot/images");

                    Directory.CreateDirectory(uploadsFolder);

                    var primaryFileName = Guid.NewGuid() + fileExtension;

                    primaryImagePath = Path.Combine(uploadsFolder, primaryFileName);

                    using (var stream = new FileStream(primaryImagePath, FileMode.Create))
                    {
                        await SubCategory.Image.CopyToAsync(stream);
                    }

                    primaryImagePath = "/images/" + primaryFileName;
                }
                catch (Exception ex)
                {
                    var categories = categoryRepository.GetAll();
                    ViewBag.category = categories;
                    ModelState.AddModelError("PrimaryImage", $"An error occurred while uploading the image : {ex.Message}");
                    return View("AddSubCategory", SubCategory);
                }
            }

            var cat = new MSubCategory
            {
                Name = SubCategory.Name,
                Image = primaryImagePath, 
                Description = SubCategory.Description,
                categoryId = SubCategory.CategoryId
            };

            try
            {
                subCategoryRepository.AddSubCategory(cat);
            }
            catch (Exception ex)
            {
                var categories = categoryRepository.GetAll();
                ViewBag.category = categories;
                ModelState.AddModelError("", $"An error occurred while saving data: {ex.Message}");
                return View("AddSubCategory", SubCategory);
            }

            return RedirectToAction("Index", "subcategory");
        }
        public IActionResult EditSubCategory(int id)
        {
            var subcat = subCategoryRepository.GetSubCategory(id);
            var categories = categoryRepository.GetAll();
            ViewBag.category = categories;
            var subcatVM = new EditSubCategoryVM()
            {
                id = subcat.Id,
                name = subcat.Name,
                description = subcat.Description,
                categoryId = subcat.categoryId,
                CurrentImage = subcat.Image
            };
            return View(subcatVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSubCategory(int id, EditSubCategoryVM Subcategory)
        {
            if (id != Subcategory.id)
            {
                var categories = categoryRepository.GetAll();
                ViewBag.category = categories;
                ModelState.AddModelError("", "The category id invalid");
                return View(Subcategory);
            }
            if (!ModelState.IsValid)
            {
                var categories = categoryRepository.GetAll();
                ViewBag.category = categories;
                return View(Subcategory);
            }
            var subcat = subCategoryRepository.GetSubCategory(id);
            string primaryImagePath = subcat.Image;
            Subcategory.CurrentImage = primaryImagePath;

            if (Subcategory.mainImage != null && Subcategory.mainImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(Subcategory.mainImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    var categories = categoryRepository.GetAll();
                    ViewBag.category = categories;
                    ModelState.AddModelError("Image", "Image format is not supported. Allowed formats: JPG, PNG, GIF");
                    return View(Subcategory);
                }

                var uploadsFolder = Path.Combine("wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);
                var primaryFileName = Guid.NewGuid() + fileExtension;
                primaryImagePath = Path.Combine(uploadsFolder, primaryFileName);


                using (var stream = new FileStream(primaryImagePath, FileMode.Create))
                {
                    await Subcategory.mainImage.CopyToAsync(stream);
                }

                primaryImagePath = "/images/" + primaryFileName;

                if (!string.IsNullOrEmpty(Subcategory.CurrentImage))
                {
                    var oldImagePath = Path.Combine("wwwroot", Subcategory.CurrentImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            var updatedSubCategory = new MSubCategory
            {                
                Name = Subcategory.name,
                Description = Subcategory.description,
                Image = primaryImagePath,
                categoryId = Subcategory.categoryId                
            };

            try
            {
                subCategoryRepository.UpdateSubCategory(id,updatedSubCategory);
            }
            catch (Exception ex)
            {
                var categories = categoryRepository.GetAll();
                ViewBag.category = categories;
                ModelState.AddModelError("", $"An error occurred while updating the category: {ex.Message}");
                return View(Subcategory);
            }

            return RedirectToAction("Index", "subcategory");
        }
        public IActionResult Delete(int id)
        {
            var subcat = subCategoryRepository.GetSubCategory(id);
            if (!string.IsNullOrEmpty(subcat.Image))
            {
                var oldImagePath = Path.Combine("wwwroot", subcat.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            subCategoryRepository.DeleteSubCategory(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
