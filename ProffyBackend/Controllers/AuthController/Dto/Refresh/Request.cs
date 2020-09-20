using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.AuthController.Dto.Refresh
{
    public class Request
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required] [PasswordPropertyText] public string Password { get; set; }
    }
}