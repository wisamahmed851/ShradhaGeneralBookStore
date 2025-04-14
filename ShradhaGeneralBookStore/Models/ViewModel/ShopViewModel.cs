using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class ShopViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Subcategory> Subcategories { get; set; }
        public List<Product> Products { get; set; }

        public Dictionary<int, List<Subcategory>> SubcategoriesByCategory { get; set; } // NEW
        public string SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
