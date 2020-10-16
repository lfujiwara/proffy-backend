using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.AvailableTimeWindowController.Dto
{
    public class DeleteDto
    {
        [Required]
        public int AvailableTimeWindowId { get; set; }
    }
}