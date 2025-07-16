namespace OnlineStore.ViewModels.Account;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CreateUserWithRoleViewModel
{
    [Required(ErrorMessage = "Please enter a name.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be at least 2 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter an email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string email { get; set; }

    [Required]
    public string phone { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
    public string confirmPassword { get; set; }


    
    [Required(ErrorMessage = "Please select a role.")]
    [Display(Name = "Role")]
    public string SelectedRole { get; set; }

    public IEnumerable<SelectListItem>? AllRoles { get; set; }
}