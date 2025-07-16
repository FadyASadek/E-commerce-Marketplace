using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string MainImage { get; set; } = string.Empty;
        public ICollection<ProductImages> Images { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey(nameof(subCategory))]
        public int supCategoryId { get; set; }
        public MSubCategory subCategory { get; set; }
    }
}
