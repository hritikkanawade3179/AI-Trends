using AITrendsNews.Data;
using AITrendsNews.Models;
using AITrendsNews.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AITrendsNews.Services
{
    public interface IPostService
    {
        Task<PagedResult<Post>> GetPublishedAsync(int page, int pageSize, PostType? type, int? categoryId, string? search, string? tag);
        Task<Post?> GetBySlugAsync(string slug);
        Task<List<Post>> GetFeaturedAsync(int count = 3);
        Task<List<Post>> GetTrendingAsync(int count = 5);
        Task<List<Post>> GetLatestAsync(int count = 10);
        Task<Post> CreateAsync(Post post, List<int> tagIds);
        Task<Post> UpdateAsync(Post post, List<int> tagIds);
        Task DeleteAsync(int id);
        Task IncrementViewAsync(int id);
        Task<DashboardStats> GetDashboardStatsAsync();
    }

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _db;
        public PostService(ApplicationDbContext db) => _db = db;

        public async Task<PagedResult<Post>> GetPublishedAsync(int page, int pageSize, PostType? type, int? categoryId, string? search, string? tag)
        {
            var q = _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Where(p => p.Status == PostStatus.Published);

            if (type.HasValue) q = q.Where(p => p.Type == type.Value);
            if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(p => p.Title.Contains(search) || p.Summary.Contains(search));
            if (!string.IsNullOrWhiteSpace(tag))
                q = q.Where(p => p.PostTags.Any(pt => pt.Tag!.Slug == tag));

            var total = await q.CountAsync();
            var items = await q.OrderByDescending(p => p.PublishedAt)
                               .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Post> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public async Task<Post?> GetBySlugAsync(string slug) =>
            await _db.Posts
                .Include(p => p.Category).Include(p => p.Author)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == PostStatus.Published);

        public async Task<List<Post>> GetFeaturedAsync(int count = 3) =>
            await _db.Posts.Include(p => p.Category)
                .Where(p => p.IsFeatured && p.Status == PostStatus.Published)
                .OrderByDescending(p => p.PublishedAt).Take(count).ToListAsync();

        public async Task<List<Post>> GetTrendingAsync(int count = 5) =>
            await _db.Posts.Include(p => p.Category)
                .Where(p => p.IsTrending && p.Status == PostStatus.Published)
                .OrderByDescending(p => p.ViewCount).Take(count).ToListAsync();

        public async Task<List<Post>> GetLatestAsync(int count = 10) =>
            await _db.Posts.Include(p => p.Category).Include(p => p.Author)
                .Where(p => p.Status == PostStatus.Published)
                .OrderByDescending(p => p.PublishedAt).Take(count).ToListAsync();

        public async Task<Post> CreateAsync(Post post, List<int> tagIds)
        {
            post.Slug = GenerateSlug(post.Title);
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            if (post.Status == PostStatus.Published) post.PublishedAt = DateTime.UtcNow;
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            await SyncTagsAsync(post.Id, tagIds);
            return post;
        }

        public async Task<Post> UpdateAsync(Post post, List<int> tagIds)
        {
            post.UpdatedAt = DateTime.UtcNow;
            if (post.Status == PostStatus.Published && post.PublishedAt == null)
                post.PublishedAt = DateTime.UtcNow;
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
            await SyncTagsAsync(post.Id, tagIds);
            return post;
        }

        public async Task DeleteAsync(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post != null) { _db.Posts.Remove(post); await _db.SaveChangesAsync(); }
        }

        public async Task IncrementViewAsync(int id)
        {
            await _db.Posts.Where(p => p.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            return new DashboardStats
            {
                TotalPosts = await _db.Posts.CountAsync(),
                PublishedPosts = await _db.Posts.CountAsync(p => p.Status == PostStatus.Published),
                DraftPosts = await _db.Posts.CountAsync(p => p.Status == PostStatus.Draft),
                TotalViews = await _db.Posts.SumAsync(p => (long)p.ViewCount),
                TotalSubscribers = await _db.NewsletterSubscribers.CountAsync(s => s.IsActive),
                PostsThisMonth = await _db.Posts.CountAsync(p => p.CreatedAt >= new DateTime(now.Year, now.Month, 1)),
                PostsByType = await _db.Posts.GroupBy(p => p.Type)
                    .Select(g => new PostTypeCount { Type = g.Key, Count = g.Count() }).ToListAsync(),
                RecentPosts = await _db.Posts.Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedAt).Take(8).ToListAsync()
            };
        }

        private async Task SyncTagsAsync(int postId, List<int> tagIds)
        {
            var existing = await _db.PostTags.Where(pt => pt.PostId == postId).ToListAsync();
            _db.PostTags.RemoveRange(existing);
            foreach (var tid in tagIds)
                _db.PostTags.Add(new PostTag { PostId = postId, TagId = tid });
            await _db.SaveChangesAsync();
        }

        public static string GenerateSlug(string title) =>
            System.Text.RegularExpressions.Regex.Replace(title.ToLower().Trim(), @"[^a-z0-9\s-]", "")
                .Replace(" ", "-").Trim('-');
    }
}
