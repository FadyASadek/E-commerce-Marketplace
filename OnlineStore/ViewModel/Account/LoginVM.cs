using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.Account
{
    public class LoginVM
    {
        [Required]
        public string userName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;
        [Display(Name = "Remember Me!")]
        public bool RememberMe { get; set; }
    }
}
