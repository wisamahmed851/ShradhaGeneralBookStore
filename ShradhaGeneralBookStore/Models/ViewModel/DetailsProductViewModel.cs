namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class DetailsProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }   // Add this
        public int SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }  // Add this
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }  // Add this
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }  // Add this
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Version { get; set; }
        public string ProductType { get; set; }
        public int Stock { get; set; }

        public string CoverImage { get; set; }  // Add this
        public List<string> DetailImages { get; set; }  // Add this
    }
}
