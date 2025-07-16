using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.PublicInfoViewModel;
using System.Linq;

namespace OnlineStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PublicInfoController : Controller
    {
        private readonly IPublicInfoRepository publicInfoRepository;

        public PublicInfoController(IPublicInfoRepository publicInfoRepository)
        {
            this.publicInfoRepository = publicInfoRepository;
        }
        public IActionResult Index()
        {
            IEnumerable<PublicInfo> publicInfos = publicInfoRepository.GetPublicInfo();
            return View(publicInfos);
        }
        [HttpGet]
        public IActionResult EditPublicInfo(int id)
        {
            PublicInfo publicInfo = publicInfoRepository.GetPublicInfoById(id);
            if (publicInfo == null)
            {
                return RedirectToAction("Index", "Publicinfo");
            }
            EditPublicInfoVM model = new EditPublicInfoVM
            {
                id = publicInfo.Id,
                Title = publicInfo.Title,
                CurrentImage=publicInfo.Logo,
                Description = publicInfo.Description,
                Email = publicInfo.Email
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPublicInfo(int id , EditPublicInfoVM model)
        {
            if (id != model.id)
            {
                ModelState.AddModelError("", "The category id invalid");
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var publicInfoLogo = publicInfoRepository.GetPublicInfoById(id);
            string primaryImagePath = publicInfoLogo.Logo;
            model.CurrentImage = primaryImagePath;

            if (model.Logo != null && model.Logo.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(model.Logo.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("Image", "Image format is not supported. Allowed formats: JPG, PNG, GIF");
                    return View(model);
                }

                var uploadsFolder = Path.Combine("wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);
                var primaryFileName = Guid.NewGuid() + fileExtension;
                primaryImagePath = Path.Combine(uploadsFolder, primaryFileName);


                using (var stream = new FileStream(primaryImagePath, FileMode.Create))
                {
                    await model.Logo.CopyToAsync(stream);
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

            var UpdatePublicInfo = new PublicInfo
            {      
                Title = model.Title,
                Logo = primaryImagePath,
                Description = model.Description,
                Email = model.Email
            };

            try
            {
               publicInfoRepository.EditPublicInfo(id,UpdatePublicInfo);
            }
            catch (Exception ex)
            {
              
                ModelState.AddModelError("", $"An error occurred while updating the category: {ex.Message}");
                return View(model);
            }
            TempData["done"] = "done";
            return RedirectToAction("Index", "PublicInfo");

        }

    }
}
