using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.ProductViewModel
{
    public class EditProductVM
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Sub category")]
        public int Subcategory { get; set; }
        public int category { get; set; }

        public string? CurrentImage { get; set; }
        [Display(Name = "Main Image")]
        public IFormFile? MainImage { get; set; }
    }

}
