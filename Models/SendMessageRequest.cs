using Microsoft.Build.Framework;

namespace Minimal_Chat_App.Models
{
    public class SendMessageRequest
    {
        //public string MessageId { get; set; }   
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }

}
