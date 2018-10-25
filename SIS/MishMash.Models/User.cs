namespace MishMash.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MishMash.Models.Enums;

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

        public virtual ICollection<UserChannel> FollowedChannels { get; set; } = new HashSet<UserChannel>();

        public Role Role { get; set; }
    }
}
