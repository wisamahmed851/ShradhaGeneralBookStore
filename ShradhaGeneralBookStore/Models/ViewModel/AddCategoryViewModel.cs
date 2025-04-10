using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class AddCategoryViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

    }
}
