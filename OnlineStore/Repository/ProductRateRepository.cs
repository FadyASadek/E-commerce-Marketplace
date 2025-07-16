using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public class ProductRateRepository : IProductRateRepository
    {
        private readonly myContext context;

        public ProductRateRepository(myContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<ProductRate>> GetAllRatingsAsync()
        {
            return await context.productRates 
                             .Include(r => r.User)
                  .Include(r => r.product)
                  .OrderByDescending(r => r.CreateAt)
                  .ToListAsync();
        }
        public async Task AddRateAsync(ProductRate rate)
        {
            await context.productRates.AddAsync(rate);
            await context.SaveChangesAsync();
        }

        public async Task UpdateRateAsync(ProductRate rate)
        {
            context.productRates.Update(rate);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRateAsync(int rateId)
        {
            var rate = await context.productRates.FindAsync(rateId);
            if (rate != null)
            {
                context.productRates.Remove(rate);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductRate>> GetRatesForProductAsync(int productId)
        {
            return await context.productRates
                                .Where(r => r.productId == productId)
                                .Include(r => r.User)
                                .AsNoTracking()
                                .ToListAsync(); 
        }

        public async Task<ProductRate?> GetUserRatingForProductAsync(string userId, int productId)
        {
            return await context.productRates
                                .FirstOrDefaultAsync(r => r.UserId == userId && r.productId == productId); 
        }

        public async Task<double> GetAverageRatingForProductAsync(int productId)
        {
            var rates = context.productRates.Where(r => r.productId == productId);

            if (!await rates.AnyAsync()) 
            {
                return 0;
            }

            return await rates.AverageAsync(r => r.valueRate); 
        }
    }
}
