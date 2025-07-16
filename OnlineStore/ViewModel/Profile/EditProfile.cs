using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.Profile
{
    public class EditProfile
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string? Description { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string emial { get; set; } = string.Empty;

        [Display(Name = "New Profile Image")]
        public IFormFile? Image { get; set; }

        public string? CurrentImage { get; set; }
    }
}