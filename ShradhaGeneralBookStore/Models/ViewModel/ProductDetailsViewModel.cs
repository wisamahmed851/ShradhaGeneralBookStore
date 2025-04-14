using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public string CoverImageUrl { get; set; }
        public List<string> DetailImageUrls { get; set; }
    }
}
