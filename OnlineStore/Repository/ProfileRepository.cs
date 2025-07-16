using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;
using OnlineStore.ViewModel.Profile;
using Microsoft.Azure.Documents.SystemFunctions;

namespace OnlineStore.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly myContext context;

        public ProfileRepository(myContext context)
        {
            this.context = context;
        }

        public async Task<ApplicationUser> GetUserByUserNameE(string userName)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<ApplicationUser> EditProfile(string userId, ApplicationUser users)
        {
            var user = await GetUserByUserName(userId);
            user.UserName = users.UserName;
            user.Email = users.Email;   
            user.Description = users.Description;
            user.PhoneNumber = users.PhoneNumber;
            user.iamge = users.iamge;
            context.SaveChanges();
            
            return user;
        }


        public async Task<ApplicationUser> GetUserByUserName(string userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(c=>c.Id==userId);
            if (user == null)
            {
                return null;
            }
            return user; 
        }

        public ApplicationUser GetUserByUserNametest(string UserId)
        {
            return context.Users.FirstOrDefault(u => u.Id == UserId);
        }
    }
}
