using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<ApplicationUser> GetUser(string id);
        Task DeleteUser(string id);
    }
}
