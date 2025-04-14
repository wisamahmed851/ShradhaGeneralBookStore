using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class HomePageViewModel
    {
        public List<Product> RecentlyAddedProducts { get; set; }
        public List<Product> CheapestProducts { get; set; }
        public List<Product> TopCategoryProducts { get; set; }
        public List<TopCategoryViewModel> TopCategories { get; set; }
        // Future additions:
        // public List<Author> Authors { get; set; }
        // public List<Publisher> Publishers { get; set; }
        // public List<Category> Categories { get; set; }
    }

}
