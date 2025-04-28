namespace ShradhaGeneralBookStore.Models.Entities
{
    public class Feedback
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public string? AdminReply { get; set; } // Admin reply to feedback

        public bool IsReplied { get; set; } = false; // true if reply is given

        public bool IsSeenByUser { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
