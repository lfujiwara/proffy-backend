using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Controllers.AvailableTimeWindowController.Dto
{
    public class AddSelfDto
    {
        [Required] [Range(0, 23)] public int StartHour { get; set; }

        [Required] [Range(0, 24)] public int EndHour { get; set; }

        [Required] [Range(1, 7)] public int WeekDay { get; set; }
    }
}