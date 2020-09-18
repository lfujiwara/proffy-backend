using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Models
{
    public class Subject
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }

        public List<User> Teachers { get; set; }
    }
}