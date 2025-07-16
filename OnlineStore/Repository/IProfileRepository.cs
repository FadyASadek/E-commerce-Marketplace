using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using OnlineStore.Models;
using OnlineStore.ViewModel.Profile;

namespace OnlineStore.Repository
{
    public interface IProfileRepository
    {
       Task< ApplicationUser> GetUserByUserName(string UserId);
        ApplicationUser GetUserByUserNametest(string UserId);
       Task< ApplicationUser> EditProfile(string userId,  ApplicationUser user);
         Task<ApplicationUser> GetUserByUserNameE(string userName);
    }
}
