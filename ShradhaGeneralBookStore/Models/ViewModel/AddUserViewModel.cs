using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
