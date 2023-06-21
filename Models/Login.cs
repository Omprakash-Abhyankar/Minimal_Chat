using System.ComponentModel.DataAnnotations;

namespace Minimal_Chat_App.Models
{
    public class Login
    {

        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
