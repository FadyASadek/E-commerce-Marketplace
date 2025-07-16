using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.ProductViewModel
{
    public class AddProductVM
    {
        [Required]
        public string name { get; set; } = string.Empty;
        [Required]
        public string description { get; set; } = string.Empty;
        [Required]
        public decimal price { get; set; }
        [Required]
        public int quantity { get; set; }
        [Required]
        public int Subcategory { get; set; }
        [Required]
        public IFormFile mainImage { get; set; }
        public List<IFormFile>? additionalImages { get; set; }
    }
}
