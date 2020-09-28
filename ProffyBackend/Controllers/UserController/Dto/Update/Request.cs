using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ProffyBackend.Models;

namespace ProffyBackend.Controllers.UserController.Dto.Update
{
    public class Request
    {
        [MinLength(1)] public string FirstName { get; set; }

        [MinLength(1)] public string LastName { get; set; }
        
        public string Biography { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Maximum hourly rate is 1e9")]
        public int HourlyRate { get; set; }
        
        [MaxLength(4)]
        public string Currency { get; set; }

        public string? SubjectId { get; set; }
    }
}