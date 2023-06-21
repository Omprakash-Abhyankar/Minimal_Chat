using Microsoft.Build.Framework;

namespace Minimal_Chat_App.Models
{
    public class SendMessageRequest
    {
        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }

}
