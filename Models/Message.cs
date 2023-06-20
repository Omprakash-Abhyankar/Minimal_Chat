using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimal_Chat_App.Models
{
    public class Message
    {
        [Key]
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation property for the sender user
        //public Users Sender { get; set; }

        //// Navigation property for the receiver user
        //public Users Receiver { get; set; }
    }
}


