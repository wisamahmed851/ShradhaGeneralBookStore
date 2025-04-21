using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class AddFaqsCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }
    }
}
