using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class ProductImages
    {
        public int id { get; set; }
        public string image { get; set; } = string.Empty;
        [ForeignKey(nameof(product))]
        public int productId { get; set; }
        public Product product { get; set; }
    }
}
