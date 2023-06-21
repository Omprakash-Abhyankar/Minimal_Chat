using System.ComponentModel.DataAnnotations;

namespace Minimal_Chat_App.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }        
        public string Email { get; set; }
        //public string Role { get; set; }

        // Navigation property for messages sent by the user        
        //public ICollection<Message> SentMessages { get; set; }

        //// Navigation property for messages received by the user
        //public ICollection<Message> ReceivedMessages { get; set; }
    }
}
