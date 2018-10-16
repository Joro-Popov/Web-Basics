using System.Collections.Generic;

namespace IRunes.Models.Domain
{
    public class User : BaseEntity<string>
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public virtual ICollection<Album> Albums { get; set; } = new HashSet<Album>();
    }
}
