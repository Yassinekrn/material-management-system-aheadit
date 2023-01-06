using System.ComponentModel.DataAnnotations;

namespace Admin_GM.Models
{
    public class Login
    {
        public string Email { get; set; }
        [Key]
        
        public string Password { get; set; }
        
    }
}
