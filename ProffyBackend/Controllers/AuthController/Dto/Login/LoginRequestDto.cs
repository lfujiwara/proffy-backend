using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.AuthController.Dto.Login
{
    public class LoginRequestDto
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required] [PasswordPropertyText] public string Password { get; set; }

        public bool RememberMe { get; set; }
        
        public bool DontSetCookie { get; set; }
    }
}