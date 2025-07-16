using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.Category;
using OnlineStore.ViewModel.Profile;
using System.Security.Claims;

namespace OnlineStore.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileRepository profileRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IProductRepository productRepository;

        public ProfileController(IProfileRepository profileRepository ,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            , IProductRepository productRepository)
        {
            this.profileRepository = profileRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.productRepository = productRepository;
        }
        public async Task< IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
              Claim claim =  User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier);
                string id = claim.Value;
                var user = await profileRepository.GetUserByUserName(id);
                var prodeuct = productRepository.GetProducts().Where(u => u.UserId == id);
                ProfileVM vm = new ProfileVM()
                {
                    username = user.UserName,
                    email = user.Email,
                    phone=user.PhoneNumber,
                    Discripation = user.Description ,
                    products = prodeuct,
                    image =user.iamge,
                };
                return View(vm);
            }
                return View("Guest");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var viewModel = new EditProfile
            {
                UserName = user.UserName,
                emial = user.Email,
                phone = user.PhoneNumber,
                Description = user.Description,
                CurrentImage = user.iamge,
            };

            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfile model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userToUpdate = await userManager.GetUserAsync(User);
            if (userToUpdate == null)
            {
                return NotFound("User not found.");
            }

            if (userToUpdate.UserName != model.UserName)
            {
                var existingUser = await userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "This username is already taken. Please choose another one.");
                    return View(model);
                }
            }

            string imagePath = userToUpdate.iamge;
            if (model.Image != null)
            {
                var uploadsFolder = Path.Combine("wwwroot", "images");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Directory.CreateDirectory(uploadsFolder); 

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(userToUpdate.iamge))
                {
                    var oldImagePath = Path.Combine("wwwroot", userToUpdate.iamge.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                imagePath = "/images/" + uniqueFileName;
            }

            userToUpdate.UserName = model.UserName;
            userToUpdate.Email = model.emial;
            userToUpdate.PhoneNumber = model.phone;
            userToUpdate.Description = model.Description;
            userToUpdate.iamge = imagePath;

            var result = await userManager.UpdateAsync(userToUpdate);

            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(userToUpdate);
                TempData["SuccessMessage"] = "Profile has been updated successfully!";
                return RedirectToAction("Index", "Profile"); 
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
