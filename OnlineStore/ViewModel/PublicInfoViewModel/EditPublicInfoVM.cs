using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.PublicInfoViewModel
{
    public class EditPublicInfoVM
    {
        public int id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Description { get; set; }

        public string? CurrentImage { get; set; }

        public IFormFile? Logo { get; set; }
    }
}
