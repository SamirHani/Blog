using Blog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Blog.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Post-Comment relationship
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade at DB level

            // Configure Post-Like relationship
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Likes)
                .WithOne(l => l.Post)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade at DB level

            // Configure ApplicationUser relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Likes)
                .WithOne(l => l.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.posts)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Subscriptions)
                .WithOne() // You may need to specify further relationships if required
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscription>()
                        .HasOne(s => s.Follower)
                        .WithMany(u => u.FollowedSubscriptions) // Users this user follows
                        .HasForeignKey(s => s.FollowerId)
                        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete on FollowerId

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Followed)
                .WithMany(u => u.FollowerSubscriptions) // Users who follow this user
                .HasForeignKey(s => s.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
