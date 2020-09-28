using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ProffyBackend.Models;

namespace ProffyBackend.Controllers.UserController.Dto.Create
{
    public class Request
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        [MinLength(10)]
        public string Password { get; set; }

        [Required] [MinLength(1)] public string FirstName { get; set; }

        [Required] [MinLength(1)] public string LastName { get; set; }
        
        public string Biography { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Maximum hourly rate is 1e9")]
        public int HourlyRate { get; set; }
        
        [MaxLength(4)]
        public string Currency { get; set; }

        public string? SubjectId { get; set; }

        [Required] [Phone] public string PhoneNumber { get; set; }

        public User ToUser()
        {
            return new User
            {
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                HourlyRate = HourlyRate,
                SubjectId = SubjectId,
                PhoneNumber = PhoneNumber
            };
        }
    }
}