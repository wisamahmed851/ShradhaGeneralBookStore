using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, StringLength(2)]
        public string Code { get; set; }

        public ICollection<Subcategory> Subcategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
