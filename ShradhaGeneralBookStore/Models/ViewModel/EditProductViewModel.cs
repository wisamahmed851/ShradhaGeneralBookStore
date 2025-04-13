using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class EditProductViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SubcategoryId { get; set; }

        public int? AuthorId { get; set; }

        public int? PublisherId { get; set; }

        [Required]
        [Range(0.01, 999999)]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [MaxLength(50)]
        public string Version { get; set; }

        [MaxLength(100)]
        public string ProductType { get; set; }

        [Required]
        public int Stock { get; set; }
        public IFormFile? coverImage { get; set; }  // Make nullable
        public List<IFormFile>? detailImages { get; set; }  // Also nullable

    }
}
