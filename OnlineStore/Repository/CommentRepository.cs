using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly myContext _context; 

        public CommentRepository(myContext context)
        {
            _context = context;
        }

        public async Task AddCommentAsync(Commment comment)
        {
            await _context.comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        

        public async Task<IEnumerable<Commment>> GetAllCommentsAsync()
        {
           return await _context.comments.Include(c=>c.User).Include(c=>c.product)
                .OrderByDescending(c=>c.CreateAt).ToListAsync();
        }

        public async Task<IEnumerable<Commment>> GetCommentsByProductIdAsync(int productId)
        {
            return await _context.comments
                                 .Where(c => c.productId == productId)
                                 .Include(c => c.User) 
                                 .OrderByDescending(c => c.CreateAt) 
                                 .ToListAsync();
        }
        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.comments.FindAsync(id);
            if (comment != null)
            {
                _context.comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Commment?> GetCommentByIdAsync(int id)
        {
            return await _context.comments.FirstOrDefaultAsync(c=>c.Id==id);
        }

        public async Task UpdateCommentAsync(Commment comment)
        {
            _context.comments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}