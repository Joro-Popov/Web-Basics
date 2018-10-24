namespace MishMash.Models.ViewModels.Channels
{
    public class FollowedViewModel
    {
        public int Index { get; set; }

        public int ChannelId { get; set; }

        public string Name { get; set; }

        public string ChannelType { get; set; }

        public int FollowersCount { get; set; }
    }
}
