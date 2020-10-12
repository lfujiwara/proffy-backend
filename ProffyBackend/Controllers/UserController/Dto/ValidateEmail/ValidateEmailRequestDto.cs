using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.UserController.Dto.ValidateEmail
{
    public class ValidateEmailRequestDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}