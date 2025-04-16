using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }  // Assuming you're using Identity

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
