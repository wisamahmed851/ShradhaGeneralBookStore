namespace ShradhaGeneralBookStore.Dtos
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string AuthorName { get; set; }
        public string PublisherName { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Version { get; set; }
        public int Stock { get; set; }
        public string status { get; set; }
        public string CoverImageUrl { get; set; }
        public List<string> DetailImageUrls { get; set; }

    }
}
