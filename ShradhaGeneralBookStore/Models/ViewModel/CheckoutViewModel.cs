using ShradhaGeneralBookStore.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class CheckoutViewModel
    {
        // From Order Table
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        public string Area { get; set; }

        public decimal DeliveryCharge { get; set; }

        public decimal ShippingFee { get; set; }


        // Extra: Display Cart Info
        public List<Cart> CartItems { get; set; }

        public decimal TotalAmount { get; set; }
    }

}
