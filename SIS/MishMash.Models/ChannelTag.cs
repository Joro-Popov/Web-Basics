namespace MishMash.Models
{
    public class ChannelTag
    {
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
    }
}
