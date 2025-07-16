using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.Comment;
using OnlineStore.ViewModel.ProductViewModel;
using OnlineStore.ViewModel.SubCategory;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace OnlineStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ISubCategoryRepository subCategoryRepository;
        private readonly IMemoryCache cache;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProductRateRepository productRateRepository;
        private readonly ICommentRepository commentRepository;

        public ProductController(IProductRepository productRepository
            ,ICategoryRepository categoryRepository,
            ISubCategoryRepository subCategoryRepository,
            IMemoryCache cache,
            UserManager<ApplicationUser> userManager,
            IProductRateRepository productRateRepository,
            ICommentRepository commentRepository
            )
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.subCategoryRepository = subCategoryRepository;
            this.cache = cache;
            this.userManager = userManager;
            this.productRateRepository = productRateRepository;
            this.commentRepository = commentRepository;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<Product> products = productRepository.GetProducts();
            return View(products);
        }
        [Authorize(Roles = "Admin , Seller")]
        [HttpGet]
        public IActionResult AddProduct()
        {
            IEnumerable<MainCategory> categories = categoryRepository.GetAll();
            ViewBag.Categories = categories;
            return View();
        }
        [Authorize(Roles = "Admin , Seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddProductVM model)
        {
            var uploadsFolder = Path.Combine("wwwroot/images");
            var additionalImagePaths = new List<string>();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = categoryRepository.GetAll();
                return View("AddProduct", model);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            string primaryImagePath = null;

            if (model.mainImage != null && model.mainImage.Length > 0)
            {
                var fileExtension = Path.GetExtension(model.mainImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ViewBag.Categories = categoryRepository.GetAll();
                    ModelState.AddModelError("mainImage", "Image format is not supported. Allowed formats: JPG, PNG, GIF");
                    return View("AddProduct", model);
                }

                try
                {
                    Directory.CreateDirectory(uploadsFolder);

                    var primaryFileName = Guid.NewGuid() + fileExtension;
                    primaryImagePath = Path.Combine(uploadsFolder, primaryFileName);

                    using (var stream = new FileStream(primaryImagePath, FileMode.Create))
                    {
                        await model.mainImage.CopyToAsync(stream);
                    }

                    primaryImagePath = "/images/" + primaryFileName;
                }
                catch (Exception ex)
                {
                    ViewBag.Categories = categoryRepository.GetAll();
                    ModelState.AddModelError("mainImage", $"An error occurred while uploading the image: {ex.Message}");
                    return View("AddProduct", model);
                }
            }

            if (model.additionalImages == null || model.additionalImages.Count < 2 || model.additionalImages.Count > 6)
            {
                ViewBag.Categories = categoryRepository.GetAll();
                ModelState.AddModelError("additionalImages", "You must upload between 2 and 6 images.");
                return View("AddProduct", model);
            }

            using (var transaction = productRepository.BeginTransaction())
            {
                try
                {
                    var newProduct = new Product
                    {
                        Name = model.name,
                        MainImage = primaryImagePath,
                        Description = model.description,
                        Price = (double)model.price,
                        Quantity = model.quantity,
                        supCategoryId = model.Subcategory,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        CreateAt = DateTime.Now
                    };

                    productRepository.AddProduct(newProduct);

                    foreach (var image in model.additionalImages)
                    {
                        var fileExtension = Path.GetExtension(image.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            throw new Exception("One or more images have an unsupported format. Allowed formats: JPG, PNG, GIF.");
                        }

                        var additionalFileName = Guid.NewGuid() + fileExtension;
                        var additionalImagePath = Path.Combine(uploadsFolder, additionalFileName);

                        using (var stream = new FileStream(additionalImagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        additionalImagePath = "/images/" + additionalFileName;
                        additionalImagePaths.Add(additionalImagePath);

                        var productImage = new ProductImages
                        {
                            image = additionalImagePath,
                            productId = newProduct.Id
                        };
                        productRepository.AddProductImage(productImage);
                        clearCache();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); 
                    ViewBag.Categories = categoryRepository.GetAll();
                    ModelState.AddModelError("additionalImages", $"An error occurred while uploading additional images");
                    return View("AddProduct", model);
                }
            }
            if (User.IsInRole("Admin"))
            {
                
            return RedirectToAction("Index", "product");
            }
            else
            {
                return RedirectToAction("Index", "profile");

            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin , Seller")]
        public IActionResult EditProduct(int id)
        {
           
            var oldProduct = productRepository.GetProductById(id);
            if (oldProduct == null)
            {
                return RedirectToAction("index","home"); 
            }
            if (User.IsInRole("Seller"))
            {
                var currentUserId = userManager.GetUserId(User);
                if (oldProduct.UserId != currentUserId)
                {
                    return Forbid();
                }
            }
            var model = new EditProductVM
            {     
                id=oldProduct.Id,
                Name = oldProduct.Name,
                Description = oldProduct.Description,
                Price = (decimal)oldProduct.Price,
                Quantity = oldProduct.Quantity,
                Subcategory = oldProduct.supCategoryId,
                CurrentImage = oldProduct.MainImage,
                category = oldProduct.subCategory.categoryId
            };
            IEnumerable<MainCategory> categories = categoryRepository.GetAll();
            ViewBag.Categories = categories;
            IEnumerable<MSubCategory> Sub = subCategoryRepository.
                GetAll().Where(s=>s.categoryId==oldProduct.subCategory.categoryId);
            ViewBag.sub = Sub;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin , Seller")]
        public async Task<IActionResult> EditProduct(int id, EditProductVM model)
        {
           
            if (!ModelState.IsValid)
            {
                IEnumerable<MainCategory> categories = categoryRepository.GetAll();
                ViewBag.Categories = categories;
                IEnumerable<MSubCategory> Sub = subCategoryRepository.
                    GetAll().Where(s => s.categoryId == model.category);
                ViewBag.sub = Sub;
                return View(model);
            }
            var subcat = productRepository.GetProductById(id);
            string primaryImagePath = subcat.MainImage;
            model.CurrentImage = primaryImagePath;

            if (model.MainImage != null && model.MainImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(model.MainImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    IEnumerable<MainCategory> categories = categoryRepository.GetAll();
                    ViewBag.Categories = categories;
                    IEnumerable<MSubCategory> Sub = subCategoryRepository.
                        GetAll().Where(s => s.categoryId == model.category);
                    ViewBag.sub = Sub;
                    ModelState.AddModelError("Image", "Image format is not supported. Allowed formats: JPG, PNG, GIF");
                    return View(model);
                }

                var uploadsFolder = Path.Combine("wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);
                var primaryFileName = Guid.NewGuid() + fileExtension;
                primaryImagePath = Path.Combine(uploadsFolder, primaryFileName);


                using (var stream = new FileStream(primaryImagePath, FileMode.Create))
                {
                    await model.MainImage.CopyToAsync(stream);
                }

                primaryImagePath = "/images/" + primaryFileName;

                if (!string.IsNullOrEmpty(model.CurrentImage))
                {
                    var oldImagePath = Path.Combine("wwwroot", model.CurrentImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            var updatedproduct = new Product
            {
                Id=id,
                Name = model.Name,
                MainImage = primaryImagePath,
                Description = model.Description,
                Price = (double)model.Price,
                Quantity = model.Quantity,
                supCategoryId = model.Subcategory,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                LastUpdate= DateTime.Now,
            };

            try
            {
                productRepository.UpdateProduct(id,updatedproduct);
                clearCache();

            }
            catch (Exception ex)
            {
                IEnumerable<MainCategory> categories = categoryRepository.GetAll();
                ViewBag.Categories = categories;
                IEnumerable<MSubCategory> Sub = subCategoryRepository.
                    GetAll().Where(s => s.categoryId == model.category);
                ViewBag.sub = Sub;
                ModelState.AddModelError("", $"An error occurred while updating the category: {ex.Message}");
                return View(model);
            }

            if (User.IsInRole("Admin"))
            {

                return RedirectToAction("Index", "product");
            }
            else
            {
                return RedirectToAction("Index", "profile");

            }
        }
        [Authorize(Roles = "Admin , Seller")]
        public IActionResult Delete(int id, string returnUrl = null)
        {
           
            
            productRepository.DeleteProduct(id);
            clearCache();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (User.IsInRole("Admin"))
            {

                return RedirectToAction("Index", "product");
            }
            else
            {
                return RedirectToAction("Index", "profile");

            }
        }
        [Authorize]
        public IActionResult GetsubcategoryByCategoryId(int id)
        {
            IEnumerable<MSubCategory> subCategories = subCategoryRepository.GetSubCategoriesByCategortId(id);
            return Json(subCategories);
        }
        public void clearCache()
        {
            var totalProduct = productRepository.GetProductCount();
            var totalPage = (int)Math.Ceiling(totalProduct / (double)10) ;
            for (int i = 1; i <= totalPage; i++)
            {
                string cacheKey = $"ProductsCache_Page_{i}";
                cache.Remove(cacheKey);
            }
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = productRepository.GetProductsBysubCategryId(product.supCategoryId);

            var comments = await commentRepository.GetCommentsByProductIdAsync(id);

            var viewModel = new detailsAndRelatedProduct
            {
                product = product,
                ListOfProdect = relatedProducts,
                AverageRating = await productRateRepository.GetAverageRatingForProductAsync(id),
                RatingCount = (await productRateRepository.GetRatesForProductAsync(id)).Count(),
                Comments = comments 
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CommentText))
            {
                return BadRequest(new { message = "Comment cannot be empty." });
            }

            var userId = userManager.GetUserId(User);

            var comment = new Commment
            {
                Comments = model.CommentText,
                productId = model.ProductId,
                UserId = userId,
                CreateAt = DateTime.Now
            };

            await commentRepository.AddCommentAsync(comment);

            var user = await userManager.FindByIdAsync(userId);

            return Json(new
            {
                commentText = comment.Comments,
                userName = user.UserName, 
                profilePicture = user.iamge, 
                createAt = comment.CreateAt.ToString("g")
            });
        }
    }
}
