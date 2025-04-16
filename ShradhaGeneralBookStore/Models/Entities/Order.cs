using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string OrderNumber { get; set; }

        public string UserId { get; set; } // or int, based on your User model

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        public string Area { get; set; }

        public decimal DistanceInKm { get; set; }

        public decimal DeliveryCharge { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        // Navigation
        public List<OrderItem> OrderItems { get; set; }
    }

}
