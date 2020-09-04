using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProffyBackend.Models.PropertyValidators;

namespace ProffyBackend.Models
{
    public class User
    {
        [Key] public int Id { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required] [PasswordPropertyText] public string Password { get; set; }
        
        [LocaleAttribute]
        public string Locale { get; set; }
    }
}