using System.ComponentModel.DataAnnotations;

namespace ProffyBackend.Models
{
    public class RoleAssignment
    {
        [Key] public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public Role Role { get; set; }
    }
}