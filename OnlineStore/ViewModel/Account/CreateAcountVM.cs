using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.Account
{
    public class CreateAcountVM
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;
        [Required]
        [Phone]
        public string phone { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;
        [Required]
        [Compare("password")]
        [Display(Name = "confirm Password")]
        [DataType(DataType.Password)]
        public string confirmpassword { get; set; } = string.Empty;

    }
}
