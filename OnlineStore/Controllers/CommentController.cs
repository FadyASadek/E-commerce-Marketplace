using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.Repository;
using OnlineStore.ViewModel.Comment;

namespace OnlineStore.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ICommentRepository commentRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            this._commentRepository = commentRepository;
            this._userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageComments()
        {
            var allComments = await _commentRepository.GetAllCommentsAsync();
            return View(allComments);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CommentText))
            {
                return BadRequest(new { message = "Comment cannot be empty." });
            }

            var userId = _userManager.GetUserId(User);

            var comment = new Commment
            {
                Comments = model.CommentText,
                productId = model.ProductId,
                UserId = userId,
                CreateAt = DateTime.Now
            };

            await _commentRepository.AddCommentAsync(comment);

            var user = await _userManager.FindByIdAsync(userId);

            return Json(new
            {
                commentText = comment.Comments,
                userName = user.UserName, 
                profilePicture = user.iamge,
                createAt = comment.CreateAt.ToString("g")
            });
        }

        [HttpPost] 
        [ValidateAntiForgeryToken] 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _commentRepository.DeleteCommentAsync(id);
            return RedirectToAction(nameof(ManageComments));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditComment(int id, Commment commentFromForm)
        {
            if (id != commentFromForm.Id)
            {
                return NotFound();
            }
            var originalComment = await _commentRepository.GetCommentByIdAsync(id);
            if (originalComment == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(commentFromForm.Comments))
            {
                ModelState.AddModelError("Comments", "Comment text cannot be empty.");
                return View(originalComment);
            }

            originalComment.Comments = commentFromForm.Comments;

            try
            {
                await _commentRepository.UpdateCommentAsync(originalComment);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Something wrong, try again later.");
            }

            return RedirectToAction(nameof(ManageComments));
        }
    }
}
