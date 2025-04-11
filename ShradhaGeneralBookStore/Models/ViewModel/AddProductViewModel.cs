using Microsoft.AspNetCore.Http;
using ShradhaGeneralBookStore.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class AddProductViewModel
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

        // Multiple Image Upload
        [Required]
        public List<IFormFile> ImageFiles { get; set; }
    }
}
