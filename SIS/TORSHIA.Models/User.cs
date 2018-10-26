using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TORSHIA.Models.Enums;

namespace TORSHIA.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();

        public virtual ICollection<UserTask> UserTasks { get; set; } = new HashSet<UserTask>();
    }
}
