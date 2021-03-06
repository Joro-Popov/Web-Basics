﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MishMash.Data;

namespace MishMash.Data.Migrations
{
    [DbContext(typeof(MishMashDbContext))]
    partial class MishMashDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MishMash.Models.Channel", b =>
                {
                    b.Property<int>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChannelType");

                    b.Property<string>("ChannelType");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ChannelId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("MishMash.Models.ChannelTag", b =>
                {
                    b.Property<int>("ChannelId");

                    b.Property<int>("TagId");

                    b.HasKey("ChannelId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ChannelTags");
                });

            modelBuilder.Entity("MishMash.Models.Tag", b =>
                {
                    b.Property<int>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ChannelId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("MishMash.Models.User", b =>
                {
                    b.Property<int>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<int>("Username");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("ChannelId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MishMash.Models.UserChannel", b =>
                {
                    b.Property<int>("ChannelId");

                    b.Property<int>("UserId");

                    b.HasKey("ChannelId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserChannels");
                });

            modelBuilder.Entity("MishMash.Models.ChannelTag", b =>
                {
                    b.HasOne("MishMash.Models.Channel", "Channel")
                        .WithMany("Tags")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MishMash.Models.Tag", "Tag")
                        .WithMany("ChannelTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MishMash.Models.UserChannel", b =>
                {
                    b.HasOne("MishMash.Models.Channel", "Channel")
                        .WithMany("Followers")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MishMash.Models.User", "User")
                        .WithMany("FollowedChannels")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
