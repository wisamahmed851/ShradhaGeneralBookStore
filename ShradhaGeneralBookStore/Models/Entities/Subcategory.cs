using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Subcategory
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [MaxLength(100)]
        public string ManufacturerName { get; set; }

        [Required, StringLength(5)]
        public string Code { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
