using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Minimal_Chat_App.Controllers;

namespace Minimal_Chat_App.Models
{
    //public class Message
    //{
    //    [Key]
    //    public string MessageId { get; set; }
    //    public string SenderId { get; set; }
    //    public string ReceiverId { get; set; }
    //    public string Content { get; set; }
    //    public DateTime Timestamp { get; set; }
    //}
    // Navigation property for the sender user
    //public Users Sender { get; set; }

    //// Navigation property for the receiver user
    //public Users Receiver { get; set; }

    
    public class Message
        {
        [Key]
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        //public Users Sender { get; set; }
        //public Users Receiver { get; set; }
    }

    public class EditMessageRequest
    {
        public string Content { get; set; }
    }




}



