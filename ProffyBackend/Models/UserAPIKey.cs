using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ProffyBackend.Models
{
    public class UserAPIKey
    {
        [Key] public int Id { get; set; }

        public string Description { get; set; }

        [Required] public User Owner { get; set; }

        [JsonIgnore] [Required] public string Key { get; set; }
    }
}