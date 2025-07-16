using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class Commment
    {
        public int Id { get; set; }
        public string Comments { get; set; }=string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(product))]
        public int productId { get; set; }
        public Product product { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }= string.Empty;
        public ApplicationUser User { get; set; }
    }
}
