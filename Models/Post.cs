using System.ComponentModel.DataAnnotations;

namespace AITrendsNews.Models
{
    public enum PostType { News, Research, Tool, Video, Event }
    public enum PostStatus { Draft, Published, Archived }

    public class Post
    {
        public int Id { get; set; }
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Summary { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? ExternalUrl { get; set; }
        public PostType Type { get; set; } = PostType.News;
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public bool IsFeatured { get; set; } = false;
        public bool IsTrending { get; set; } = false;
        public int ViewCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}
