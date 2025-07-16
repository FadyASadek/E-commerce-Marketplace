using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.Category
{
    public class EditCategoryVm
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Display(Name = "New Image")]
        public IFormFile? PrimaryImage { get; set; }
        public string? CurrentImage { get; set; }
    }
}
