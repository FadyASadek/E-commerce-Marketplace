using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using System.Security.Claims;

namespace OnlineStore.Repository
{
    public class AccountUserRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccountUserRepository(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

       

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var result = await userManager.Users.ToListAsync();
            return result;
        }
        public async Task<ApplicationUser> GetUser(string id)
        {
            var res = await userManager.FindByIdAsync(id);
            if (res == null)
            {
                throw new Exception ( "User Not Found" );
            }
                return res;
        }

        public async Task DeleteUser(string id)
        {
            var currentUserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == id)
            {
                throw new InvalidOperationException("You cannot delete your own account while logged in.");
            }
            var user = await GetUser(id);
            var res = await userManager.DeleteAsync(user);
           
            if (!res.Succeeded)
            {
                throw new Exception("Error in delete User");
            }
        }

        
    }
}
