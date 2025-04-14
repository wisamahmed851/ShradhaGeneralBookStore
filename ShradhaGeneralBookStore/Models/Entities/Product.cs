using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int SubcategoryId { get; set; }
        [ForeignKey("SubcategoryId")]
        public Subcategory Subcategory { get; set; }

        public int? AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

        public int? PublisherId { get; set; }
        [ForeignKey("PublisherId")]
        public Publisher Publisher { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public DateTime ReleaseDate { get; set; }

        [MaxLength(50)]
        public string Version { get; set; }

        [MaxLength(100)]
        public string ProductType { get; set; }

        [Required, StringLength(10)]
        public string UniqueCode { get; set; }

        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProductImage> ProductImages { get; set; }
        [NotMapped]
        public ProductImage CoverImage => ProductImages?.FirstOrDefault(img => img.ImageType == ProductImageType.Cover);

        [NotMapped]
        public IEnumerable<ProductImage> DetailImages => ProductImages?.Where(img => img.ImageType == ProductImageType.Detail);


    }
}
