using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Models
{
    public class Subject
    {
        [Key] public string Id { get; set; }

        public List<User> Teachers { get; set; }
    }
}