using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProffyBackend.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        [JsonIgnore]
        public string Password { get; set; }

        [Required] [MinLength(1)] public string FirstName { get; set; }

        [Required] [MinLength(1)] public string LastName { get; set; }

        public string Biography { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Maximum hourly rate is 1e9")]
        public int? HourlyRate { get; set; }

        [MaxLength(4)] public string Currency { get; set; }

        public string? SubjectId { get; set; }

        [Required] [Phone] public string PhoneNumber { get; set; }

        [Required] public string Role { get; set; }

        [JsonIgnore] public List<UserAPIKey> ApiKeys { get; set; }
    }
}