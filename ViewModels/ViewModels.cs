using AITrendsNews.Models;

namespace AITrendsNews.ViewModels
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class HomeViewModel
    {
        public List<Post> FeaturedPosts { get; set; } = new();
        public List<Post> TrendingPosts { get; set; } = new();
        public List<Post> LatestPosts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }

    public class NewsListViewModel
    {
        public PagedResult<Post> Posts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Tag> PopularTags { get; set; } = new();
        public PostType? SelectedType { get; set; }
        public int? SelectedCategory { get; set; }
        public string? Search { get; set; }
        public string? Tag { get; set; }
    }

    public class PostDetailViewModel
    {
        public Post Post { get; set; } = null!;
        public List<Post> RelatedPosts { get; set; } = new();
    }

    public class DashboardStats
    {
        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int DraftPosts { get; set; }
        public long TotalViews { get; set; }
        public int TotalSubscribers { get; set; }
        public int PostsThisMonth { get; set; }
        public List<PostTypeCount> PostsByType { get; set; } = new();
        public List<Post> RecentPosts { get; set; } = new();
    }

    public class PostTypeCount
    {
        public PostType Type { get; set; }
        public int Count { get; set; }
    }

    public class PostFormViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? ExternalUrl { get; set; }
        public PostType Type { get; set; }
        public PostStatus Status { get; set; }
        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsTrending { get; set; }
        public List<int> SelectedTagIds { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Tag> AllTags { get; set; } = new();
    }
}
