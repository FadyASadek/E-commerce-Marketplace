using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.Home;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMemoryCache _cache;
        private readonly ISubCategoryRepository subCategoryRepository;
        private readonly IProductRateRepository _rateRepo;
        private const int PageSize = 12; 

        public HomeController(IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMemoryCache cache,
            ISubCategoryRepository subCategoryRepository,
            IProductRateRepository rateRepo
            )
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            _cache = cache;
            this.subCategoryRepository = subCategoryRepository;
            this._rateRepo = rateRepo;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var allProducts = productRepository.GetPRoductForPagnation(page, PageSize, out int TotalProduct);
            var categories = categoryRepository.GetAll();

            var productViewModels = new List<ProductViewModel>();
            foreach (var product in allProducts)
            {
                productViewModels.Add(new ProductViewModel
                {
                    Product = product,
                    AverageRating = await _rateRepo.GetAverageRatingForProductAsync(product.Id),
                    RatingCount = (await _rateRepo.GetRatesForProductAsync(product.Id)).Count()
                });
            }

            var viewModel = new HomeCategoryAndProductVM
            {
                Products = productViewModels, 
                mainCategories = categories
            };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(TotalProduct / (double)PageSize);

            return View(viewModel);
        }
        [Authorize]
        public async Task< IActionResult> GetALLCategory()
        {
            
            try
            {
                var AllCategory = await categoryRepository.GetAllCategoryWithProductCount();
                 return View(AllCategory);
            }
            catch(Exception ex) {
               ModelState.AddModelError("Error! Try Again" , ex.Message);
                return View();
            }
        }
        [Authorize]
        public IActionResult GetSubCategoryByCategoryId(int Id)
        {
            var subCategories = subCategoryRepository.GetSubCategoriesByCategortId(Id);
            var products = productRepository.GetProductsByCategoryId(Id);

            if (subCategories == null || !subCategories.Any())
            {
                ViewBag.Message = "No subcategories available.";
                return View(new subcategoryAndProduct()); 
            }
            var model = new subcategoryAndProduct()
            {
                products = products,
                subCategories = subCategories
            };
            return View(model);
        }
        [Authorize]
        public IActionResult GetProductBySubCategry(int Id)
        {
            var product = productRepository.GetProductsBysubCategryId(Id);
            return View(product);
        }

    }
}
