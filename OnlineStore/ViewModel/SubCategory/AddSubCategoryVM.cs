using OnlineStore.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.SubCategory
{
    public class AddSubCategoryVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; } 

        [Display(Name = "Image")]
        public IFormFile Image { get; set; } 
        [Required(ErrorMessage = "Please select a main category.")]
        [Display(Name = "Main Category")]
        public int CategoryId { get; set; }
    }
}
