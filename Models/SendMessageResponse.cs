namespace Minimal_Chat_App.Models
{
    public class SendMessageResponse
    {
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
