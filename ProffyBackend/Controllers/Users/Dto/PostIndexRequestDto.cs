using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProffyBackend.Models.PropertyValidators;

namespace ProffyBackend.Controllers.Users.Dto
{
    public class PostIndexRequestDto
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required] [PasswordPropertyText] public string Password { get; set; }
        
        [LocaleAttribute]
        public string Locale { get; set; }
    }
}