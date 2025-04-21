using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class FaqCategory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Faq> Faqs { get; set; }
    }


}
