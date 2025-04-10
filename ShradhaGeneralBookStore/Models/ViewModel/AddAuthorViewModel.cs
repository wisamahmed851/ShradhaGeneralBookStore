using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModels
{
    public class AddAuthorViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}
