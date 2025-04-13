using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class HomePageViewModel
    {
        public List<Product> Products { get; set; }

        // In future:
        // public List<Author> Authors { get; set; }
        // public List<Publisher> Publishers { get; set; }
        // public List<Category> Categories { get; set; }
    }
}
