using System.ComponentModel.DataAnnotations;

namespace AITrendsNews.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required, MaxLength(80)]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}
