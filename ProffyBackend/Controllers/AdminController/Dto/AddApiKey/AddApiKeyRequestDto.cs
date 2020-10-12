using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.AdminController.Dto.AddApiKey
{
    public class AddApiKeyDto
    {
        [Required] [EmailAddress] public string OwnerEmail { get; set; }

        [Required] public string Description { get; set; }
    }
}