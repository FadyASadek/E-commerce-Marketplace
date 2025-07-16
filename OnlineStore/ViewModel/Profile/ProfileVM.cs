using OnlineStore.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.Profile
{
    public class ProfileVM
    {
        [Required]
        public string username { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string phone { get; set; }
        [Required]
        public string Discripation { get; set; }
        public string image { get; set; }
        public IEnumerable<Product> products { get; set; }
    }
}
