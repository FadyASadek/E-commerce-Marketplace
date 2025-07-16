using OnlineStore.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.ViewModels
{
    public class CheckoutViewModel
    {
        // --- قسم بيانات الشحن (سيتم ملؤه من الفورم) ---
        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full Name")]
        public string RecipientName { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [Display(Name = "Phone Number")]
        public string RecipientPhone { get; set; }

        [Required(ErrorMessage = "Please enter your shipping address.")]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Please enter your city.")]
        public string City { get; set; }


        // --- قسم ملخص الطلب (للعرض فقط) ---
        public IEnumerable<ShoppingCartItem>? CartItems { get; set; }
        public decimal GrandTotal { get; set; }
    }
}