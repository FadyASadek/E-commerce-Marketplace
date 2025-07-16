using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.ViewModel.Account;
using OnlineStore.ViewModel.SubCategory;
using OnlineStore.ViewModel.UserManagement;
using OnlineStore.ViewModels;
using OnlineStore.ViewModels.Account;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")] 
public class UserManagementController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager; 

    public UserManagementController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager) 
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager; 
    }

    
    [HttpGet]
    public async Task<IActionResult> CreateUserWithRole()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var model = new CreateUserWithRoleViewModel
        {
            AllRoles = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            })
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUserWithRole(CreateUserWithRoleViewModel createVM)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = createVM.Name,
                PhoneNumber = createVM.phone,
                Email = createVM.email
            };

            var result = await _userManager.CreateAsync(user, createVM.password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(createVM.SelectedRole) && await _roleManager.RoleExistsAsync(createVM.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, createVM.SelectedRole);

                    TempData["SuccessMessage"] = $"User '{user.UserName}' created successfully in role '{createVM.SelectedRole}'.";
                    return RedirectToAction("CreateUserWithRole");
                }
                else
                {
                    ModelState.AddModelError("SelectedRole", "The selected role does not exist.");
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        var roles = await _roleManager.Roles.ToListAsync();
        createVM.AllRoles = roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name });
        return View(createVM);
    }
  
    [HttpGet]
    public async Task<IActionResult> ManageUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ManageUserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = new List<RoleViewModel>()
        };

        foreach (var role in await _roleManager.Roles.ToListAsync())
        {
            var roleViewModel = new RoleViewModel
            {
                RoleName = role.Name,
                IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
            };
            model.Roles.Add(roleViewModel);
        }

        return View(model);
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in model.Roles)
        {
            if (role.IsSelected && !await _userManager.IsInRoleAsync(user, role.RoleName))
            {
                await _userManager.AddToRoleAsync(user, role.RoleName);
            }
            else if (!role.IsSelected && await _userManager.IsInRoleAsync(user, role.RoleName))
            {
                await _userManager.RemoveFromRoleAsync(user, role.RoleName);
            }
        }

        TempData["SuccessMessage"] = $"Roles for {user.UserName} updated successfully.";
        return RedirectToAction("GetAllUser", "Account");
    }
}