using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(Commment comment);
        Task<IEnumerable<Commment>> GetCommentsByProductIdAsync(int productId);
        Task<IEnumerable<Commment>> GetAllCommentsAsync();
        Task DeleteCommentAsync(int id);
        Task<Commment?> GetCommentByIdAsync(int id);
        Task UpdateCommentAsync(Commment comment);
    }
}
