namespace MishMash.App.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using MishMash.Models;
    using MishMash.Models.Enums;
    using MishMash.Models.ViewModels.Channels;
    using SIS.Framework.ActionResults.Contracts.Base;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.Attributes.Methods;
    using SIS.HTTP.Exceptions;

    public class ChannelsController : BaseController
    {
        [HttpGet]
        [Authorize]
        public IActionResult Details(int channelId)
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/users/login");
            }

            var channel = this.DbContext.Channels
                .Where(ch => ch.Id == channelId)
                .Select(ch => new DetailsViewModel()
                {
                    Channel = ch.Name,
                    Type = ch.ChannelType.ToString(),
                    Followers = ch.Followers.Count,
                    Description = ch.Description,
                    Tags = string.Join(", ", ch.Tags.Select(t => t.Tag.Name))
                }).FirstOrDefault();
            
            this.Model.Data["Details"] = channel;

            return this.View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Followed()
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/users/login");
            }

            var channels = this.DbContext.Users
                .FirstOrDefault(u => u.Username == this.Identity.Username)
                .FollowedChannels
                .Select(ch => ch.Channel)
                .Select((ch, i) => new FollowedViewModel()
                {
                    Index = i + 1,
                    ChannelId = ch.Id,
                    Name = ch.Name,
                    ChannelType = ch.ChannelType.ToString(),
                    FollowersCount = ch.Followers.Count
                }).ToList();

            this.Model.Data["Channels"] = channels;

            return this.View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Follow(int channelId)
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/users/login");
            }

            var user = this.DbContext.Users
                .FirstOrDefault(u => u.Username == this.Identity.Username);

            if (user.FollowedChannels.Any(ch => ch.ChannelId == channelId))
            {
                return this.RedirectToAction("/channels/followed");
            }

            user.FollowedChannels.Add(new UserChannel()
            {
                ChannelId = channelId,
                UserId = user.Id
            });

            try
            {
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InternalServerException(e.Message);
            }

            return this.RedirectToAction("/home/index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult UnFollow(int channelId)
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/users/login");
            }

            var user = this.DbContext.Users
                .FirstOrDefault(u => u.Username == this.Identity.Username);

            var userInChannel = this.DbContext.UserChannels
                .FirstOrDefault(u => u.UserId == user.Id && u.ChannelId == channelId);

            if (userInChannel == null)
            {
                return this.RedirectToAction("/channels/followed");
            }

            this.DbContext.UserChannels.Remove(userInChannel);

            try
            {
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InternalServerException(e.Message);
            }

            return this.RedirectToAction("/channels/followed");
        }

        [HttpGet]
        [Authorize("Admin")]
        public IActionResult Create()
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/users/login");
            }

            return this.View();
        }

        [HttpPost]
        [Authorize("Admin")]
        public IActionResult Create(CreateChannelViewModel model)
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/users/login");
            }

            var channel = new Channel()
            {
                Name = WebUtility.UrlDecode(model.Name),
                Description = WebUtility.UrlDecode(model.Description),
                ChannelType = (ChannelType) Enum.Parse(typeof(ChannelType),model.ChannelType, true)
            };

            var tags = WebUtility.UrlDecode(model.Tags).Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Select(t => new ChannelTag()
                {
                    Tag = new Tag() {Name = t},
                    Channel = channel
                }).ToList();

            channel.Tags = tags;

            var channels = this.DbContext.Channels.ToList();

            if (channels.Any(ch => ch.Name == channel.Name))
            {
                return this.View();
            }

            try
            {
                this.DbContext.Channels.Add(channel);
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InternalServerException(e.Message);
            }

            return this.RedirectToAction("/home/index");
        }
    }
}
