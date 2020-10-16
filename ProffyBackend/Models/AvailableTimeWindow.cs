using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProffyBackend.Models
{
    public class AvailableTimeWindow
    {
        [Key] public int Id { get; set; }

        [Required] [JsonIgnore] public Guid OwnerId { get; set; }
        [Required] [JsonIgnore] public User Owner { get; set; }

        [Required] [Range(0, 24)] public int StartHour { get; set; }

        [Required] [Range(0, 24)] public int EndHour { get; set; }

        [Required] [Range(1, 7)] public int WeekDay { get; set; }
    }
}