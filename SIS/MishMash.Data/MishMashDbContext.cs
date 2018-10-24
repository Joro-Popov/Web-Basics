using MishMash.Models;

namespace MishMash.Data
{
    using Microsoft.EntityFrameworkCore;

    public class MishMashDbContext : DbContext
    {
        public MishMashDbContext(DbContextOptions options) : base(options)
        {

        }

        public  MishMashDbContext()
        {
        }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ChannelTag> ChannelTags { get; set; }

        public DbSet<UserChannel> UserChannels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder
                .UseSqlServer(Configuration.ConfigurationString)
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserChannel>().HasKey(uc => new {uc.ChannelId, uc.UserId});
            modelBuilder.Entity<ChannelTag>().HasKey(ct => new {ct.ChannelId, ct.TagId});
        }
    }
}
