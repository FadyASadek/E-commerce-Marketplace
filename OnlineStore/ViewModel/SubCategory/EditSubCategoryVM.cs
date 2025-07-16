using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.SubCategory
{
    public class EditSubCategoryVM
    {
        public int id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string description { get; set; }

        public string? CurrentImage { get; set; }

        [Display(Name = "New Image")]
        public IFormFile? mainImage { get; set; } 

        [Required(ErrorMessage = "Please select the main category.")]
        [Display(Name = "Main Category")]
        public int categoryId { get; set; }
    }
}
