using System.Linq;
using MishMash.Models.ViewModels.Channels;
using MishMash.Models.ViewModels.Users;
using SIS.Framework.ActionResults.Contracts.Base;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;

namespace MishMash.App.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/authorized");
            }

            return this.View();
        }

        [HttpGet]
        [Authorize("User")]
        public IActionResult Authorized()
        {
            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == this.Identity.Username);

            var followingChannels = user.FollowedChannels
                .Select(c => new FollowChannelsViewModel()
                {
                    Name = c.Channel.Name,
                    ChannelType = $"{c.Channel.ChannelType.ToString()} Channel",
                    FollowingCount = c.Channel.Followers.Count
                })
                .ToList();
            
            var suggestedChannels = this.DbContext.Channels
                .Where(c => c.Tags.Select(t => t.Tag.Name).Any(t =>
                    user.FollowedChannels.SelectMany(fc => fc.Channel.Tags.Select(tg => tg.Tag.Name)).Contains(t)
                    && user.FollowedChannels.All(uc => uc.ChannelId != c.Id)))

                .Select(c => new SuggestedChannelsViewModel()
                {
                    ChannelId = c.Id,
                    Name = c.Name,
                    ChannelType = $"{c.ChannelType.ToString()} Channel",
                    FollowingCount = c.Followers.Count
                })
                .ToList();
            
            var otherChannels = this.DbContext.Channels
                .Where(ch => !user.FollowedChannels.Select(c => c.Channel.Id).Contains(ch.Id) &&
                             !suggestedChannels.Select(sc => sc.ChannelId).Contains(ch.Id))
                .Select(c => new OtherChannelsViewModel()
                {
                    ChannelId = c.Id,
                    Name = c.Name,
                    ChannelType = $"{c.ChannelType.ToString()} Channel",
                    FollowingCount = c.Followers.Count
                })
                .ToList();

            this.Model.Data["Username"] = this.Identity.Username;
            this.Model.Data["FollowingChannels"] = followingChannels;
            this.Model.Data["SuggestedChannels"] = suggestedChannels;
            this.Model.Data["OtherChannels"] = otherChannels;

            return this.View();
        }
    }
}
