using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } 
        [Required]
        public string CustomerId { get; set; } 
        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        [Required]
        public string RecipientName { get; set; }
        [Required]
        public string RecipientPhone { get; set; } 
        [Required]
        public string ShippingAddress { get; set; } 
        [Required]
        public string City { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal OrderTotal { get; set; } 

        public string OrderStatus { get; set; } 
        public string PaymentMethod { get; set; } 
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
