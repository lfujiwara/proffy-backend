using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.UserController.Dto.ValidatePhoneNumber
{
    public class ValidatePhoneNumberRequestDto
    {
        [Phone]
        [Required]
        public string PhoneNumber { get; set; }
    }
}