using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.UserController.Dto.ChangePassword
{
    public class Request
    {
        [Required]
        [PasswordPropertyText]
        [MinLength(10)]
        public string Password { get; set; }

        [Required]
        [PasswordPropertyText]
        [MinLength(10)]
        public string NewPassword { get; set; }
    }
}