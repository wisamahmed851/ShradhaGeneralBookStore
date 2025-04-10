using System.ComponentModel.DataAnnotations;
using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        // Make the Products collection optional, no need to vali
        public ICollection<Product> Products { get; set; }
    }

}
