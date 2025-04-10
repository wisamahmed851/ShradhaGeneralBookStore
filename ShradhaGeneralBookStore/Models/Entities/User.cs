using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
