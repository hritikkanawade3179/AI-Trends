using AITrendsNews.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AITrendsNews.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            builder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId);

            builder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId);

            builder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "LLMs & Chatbots",    Slug = "llms-chatbots",    IconClass = "bi bi-chat-dots",     ColorHex = "#534AB7", SortOrder = 1 },
                new Category { Id = 2, Name = "Computer Vision",    Slug = "computer-vision",  IconClass = "bi bi-eye",           ColorHex = "#0F6E56", SortOrder = 2 },
                new Category { Id = 3, Name = "Research Papers",    Slug = "research-papers",  IconClass = "bi bi-journal-text",  ColorHex = "#185FA5", SortOrder = 3 },
                new Category { Id = 4, Name = "AI Tools",           Slug = "ai-tools",         IconClass = "bi bi-tools",         ColorHex = "#854F0B", SortOrder = 4 },
                new Category { Id = 5, Name = "Video Updates",      Slug = "video-updates",    IconClass = "bi bi-play-circle",   ColorHex = "#993C1D", SortOrder = 5 },
                new Category { Id = 6, Name = "Events & Conferences",Slug = "events",           IconClass = "bi bi-calendar-event",ColorHex = "#3B6D11", SortOrder = 6 }
            );
        }
    }
}
