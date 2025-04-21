namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Feedback
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
