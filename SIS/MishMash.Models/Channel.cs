using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MishMash.Models.Enums;

namespace MishMash.Models
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ChannelType ChannelType { get; set; }

        public virtual ICollection<ChannelTag> Tags { get; set; }

        public virtual ICollection<UserChannel> Followers { get; set; } = new HashSet<UserChannel>();
    }
}
