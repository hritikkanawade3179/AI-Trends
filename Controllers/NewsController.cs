using AITrendsNews.Data;
using AITrendsNews.Models;
using AITrendsNews.Services;
using AITrendsNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AITrendsNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly IPostService _posts;
        private readonly ApplicationDbContext _db;

        public NewsController(IPostService posts, ApplicationDbContext db)
        {
            _posts = posts;
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, PostType? type = null,
            int? categoryId = null, string? search = null, string? tag = null)
        {
            var vm = new NewsListViewModel
            {
                Posts = await _posts.GetPublishedAsync(page, 12, type, categoryId, search, tag),
                Categories = await _db.Categories.OrderBy(c => c.SortOrder).ToListAsync(),
                PopularTags = await _db.Tags
                    .OrderByDescending(t => t.PostTags.Count).Take(20).ToListAsync(),
                SelectedType = type,
                SelectedCategory = categoryId,
                Search = search,
                Tag = tag
            };
            return View(vm);
        }

        public async Task<IActionResult> Detail(string slug)
        {
            var post = await _posts.GetBySlugAsync(slug);
            if (post == null) return NotFound();

            await _posts.IncrementViewAsync(post.Id);

            var related = await _db.Posts
                .Include(p => p.Category)
                .Where(p => p.CategoryId == post.CategoryId
                         && p.Status == PostStatus.Published
                         && p.Id != post.Id)
                .OrderByDescending(p => p.PublishedAt)
                .Take(4).ToListAsync();

            return View(new PostDetailViewModel { Post = post, RelatedPosts = related });
        }
    }
}
