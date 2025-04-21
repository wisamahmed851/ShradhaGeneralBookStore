using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class AddFaqViewModel
    {
        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        public int CategoryId { get; set; }
    }
}
