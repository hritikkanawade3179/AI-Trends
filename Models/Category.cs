using System.ComponentModel.DataAnnotations;

namespace AITrendsNews.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public string? ColorHex { get; set; }
        public int SortOrder { get; set; } = 0;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
