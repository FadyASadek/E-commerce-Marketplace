using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class ProductRate
    {
        public int Id { get; set; }
        public int valueRate { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }
        [ForeignKey(nameof(product))]
        public int productId { get; set; }
        public Product product { get; set; }

    }
}
