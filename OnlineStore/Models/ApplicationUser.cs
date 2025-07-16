using Microsoft.AspNetCore.Identity;

namespace OnlineStore.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string iamge { get; set; } = string.Empty;
        public ICollection<Product> products { get; set; }
        public ICollection<Commment> comments { get; set; }
        public ICollection<ProductRate> productRates { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
