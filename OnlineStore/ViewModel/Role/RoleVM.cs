using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModel.Role
{
    public class RoleVM
    {
        [Required(ErrorMessage = "Role name is required.")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
