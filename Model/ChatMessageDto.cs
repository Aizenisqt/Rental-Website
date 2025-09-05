namespace RentalSite.Pages.Model
{
    public class ChatMessageDto
    {
        public int Id { get; set; }          // message id
        public string Content { get; set; }  // message text
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string ProfilePhoto { get; set; } // comes from users table
        public DateTime Timestamp { get; set; }
    }
}
