using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public interface IProductRateRepository
    {
        
        Task AddRateAsync(ProductRate rate);
        Task UpdateRateAsync(ProductRate rate);
        Task DeleteRateAsync(int rateId);
        Task<IEnumerable<ProductRate>> GetRatesForProductAsync(int productId);     
        Task<ProductRate?> GetUserRatingForProductAsync(string userId, int productId);      
        Task<double> GetAverageRatingForProductAsync(int productId);
        Task<IEnumerable<ProductRate>> GetAllRatingsAsync();

    }
}
