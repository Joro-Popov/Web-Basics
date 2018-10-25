using Microsoft.EntityFrameworkCore;
using TORSHIA.Models;

namespace TORSHIA.Data
{
    public class TorshiaDbContext : DbContext
    {
        public TorshiaDbContext(DbContextOptions options) : base(options)
        {
        }

        public TorshiaDbContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<AffectedSector> AffectedSectors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder
                .UseSqlServer(Configuration.CONNECTION_STRING_S)
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>().HasKey(x => new {x.TaskId, x.UserId});
        }
    }
}
