using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.Category
{
    public class CategoryVM
    {    
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public IFormFile primaryImage { get; set; }
    }
}
