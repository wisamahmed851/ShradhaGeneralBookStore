using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Faq
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("FaqCategory")]
        public int CategoryId { get; set; }

        public FaqCategory Category { get; set; }
    }


}
