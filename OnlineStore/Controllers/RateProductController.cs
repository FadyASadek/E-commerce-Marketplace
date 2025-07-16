using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Repository;

namespace OnlineStore.Controllers
{
    [Authorize]
    public class RateProductController : Controller
    {
        private readonly IProductRateRepository _productRateRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RateProductController(IProductRateRepository productRateRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            this._productRateRepository = productRateRepository;
            this._userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRatings()
        {
            var allRatings = await _productRateRepository.GetAllRatingsAsync();
            return View(allRatings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRating(int id)
        {
           
            await _productRateRepository.DeleteRateAsync(id);
            return RedirectToAction(nameof(ManageRatings));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRating([FromBody] RatingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Error." });
            }

            var userId = _userManager.GetUserId(User); 

            var existingRate = await _productRateRepository.GetUserRatingForProductAsync(userId, model.ProductId);

            if (existingRate != null)
            {
                existingRate.valueRate = model.RatingValue;
                existingRate.CreateAt = DateTime.Now;
                await _productRateRepository.UpdateRateAsync(existingRate);
            }
            else
            {
                var productRate = new ProductRate
                {
                    productId = model.ProductId,
                    valueRate = model.RatingValue,
                    UserId = userId,
                    CreateAt = DateTime.Now
                };
                await _productRateRepository.AddRateAsync(productRate);
            }

            var newAverage = await _productRateRepository.GetAverageRatingForProductAsync(model.ProductId);
            var reviewCount = (await _productRateRepository.GetRatesForProductAsync(model.ProductId)).Count();

            return Ok(new
            {
                message = "Thank you for your rating!",
                newAverageRating = newAverage,
                reviewCount = reviewCount
            });
        }
    }
}
