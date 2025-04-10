using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class AddSubcategoryViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        [MaxLength(100)]
        public string ManufacturerName { get; set; }

        
    }
}
