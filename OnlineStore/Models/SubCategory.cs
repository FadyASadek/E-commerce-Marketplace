using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class MSubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        [ForeignKey(nameof(category))]
        public int categoryId { get; set; }
        public MainCategory category { get; set; }
        public ICollection<Product> products { get; set; }

    }
}
