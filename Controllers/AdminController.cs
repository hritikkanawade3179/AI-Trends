using AITrendsNews.Data;
using AITrendsNews.Models;
using AITrendsNews.Services;
using AITrendsNews.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AITrendsNews.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IPostService _posts;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _users;

        public AdminController(IPostService posts, ApplicationDbContext db, UserManager<ApplicationUser> users)
        {
            _posts = posts; _db = db; _users = users;
        }

        [HttpGet("")]
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _posts.GetDashboardStatsAsync();
            return View(stats);
        }

        // ---- Posts ----
        [HttpGet("posts")]
        public async Task<IActionResult> Posts(int page = 1, string? search = null, PostStatus? status = null)
        {
            var q = _db.Posts.Include(p => p.Category).Include(p => p.Author).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(p => p.Title.Contains(search));
            if (status.HasValue) q = q.Where(p => p.Status == status.Value);

            var total = await q.CountAsync();
            var items = await q.OrderByDescending(p => p.CreatedAt)
                               .Skip((page - 1) * 15).Take(15).ToListAsync();
            ViewBag.Total = total; ViewBag.Page = page; ViewBag.Search = search; ViewBag.Status = status;
            return View(items);
        }

        [HttpGet("posts/create")]
        public async Task<IActionResult> CreatePost()
        {
            return View(await BuildPostFormAsync(null));
        }

        [HttpPost("posts/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostFormViewModel vm)
        {
            if (!ModelState.IsValid) { vm = await BuildPostFormAsync(vm); return View(vm); }
            var user = await _users.GetUserAsync(User);
            var post = new Post
            {
                Title = vm.Title, Summary = vm.Summary, Body = vm.Body,
                ThumbnailUrl = vm.ThumbnailUrl, ExternalUrl = vm.ExternalUrl,
                Type = vm.Type, Status = vm.Status, CategoryId = vm.CategoryId,
                IsFeatured = vm.IsFeatured, IsTrending = vm.IsTrending,
                AuthorId = user!.Id
            };
            await _posts.CreateAsync(post, vm.SelectedTagIds);
            TempData["Success"] = "Post created successfully.";
            return RedirectToAction(nameof(Posts));
        }

        [HttpGet("posts/edit/{id}")]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _db.Posts.Include(p => p.PostTags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();
            var vm = await BuildPostFormAsync(null);
            vm.Id = post.Id; vm.Title = post.Title; vm.Summary = post.Summary;
            vm.Body = post.Body; vm.ThumbnailUrl = post.ThumbnailUrl;
            vm.ExternalUrl = post.ExternalUrl; vm.Type = post.Type;
            vm.Status = post.Status; vm.CategoryId = post.CategoryId;
            vm.IsFeatured = post.IsFeatured; vm.IsTrending = post.IsTrending;
            vm.SelectedTagIds = post.PostTags.Select(pt => pt.TagId).ToList();
            return View(vm);
        }

        [HttpPost("posts/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, PostFormViewModel vm)
        {
            if (!ModelState.IsValid) { vm = await BuildPostFormAsync(vm); return View(vm); }
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();
            post.Title = vm.Title; post.Summary = vm.Summary; post.Body = vm.Body;
            post.ThumbnailUrl = vm.ThumbnailUrl; post.ExternalUrl = vm.ExternalUrl;
            post.Type = vm.Type; post.Status = vm.Status; post.CategoryId = vm.CategoryId;
            post.IsFeatured = vm.IsFeatured; post.IsTrending = vm.IsTrending;
            await _posts.UpdateAsync(post, vm.SelectedTagIds);
            TempData["Success"] = "Post updated successfully.";
            return RedirectToAction(nameof(Posts));
        }

        [HttpPost("posts/delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _posts.DeleteAsync(id);
            TempData["Success"] = "Post deleted.";
            return RedirectToAction(nameof(Posts));
        }

        // ---- Categories ----
        [HttpGet("categories")]
        public async Task<IActionResult> Categories()
        {
            var cats = await _db.Categories
                .Include(c => c.Posts).OrderBy(c => c.SortOrder).ToListAsync();
            return View(cats);
        }

        [HttpPost("categories/save")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveCategory(Category cat)
        {
            if (cat.Id == 0)
            {
                cat.Slug = PostService.GenerateSlug(cat.Name);
                _db.Categories.Add(cat);
            }
            else
            {
                _db.Categories.Update(cat);
            }
            await _db.SaveChangesAsync();
            TempData["Success"] = "Category saved.";
            return RedirectToAction(nameof(Categories));
        }

        // ---- Tags ----
        [HttpGet("tags")]
        public async Task<IActionResult> Tags()
        {
            var tags = await _db.Tags.Include(t => t.PostTags).OrderBy(t => t.Name).ToListAsync();
            return View(tags);
        }

        [HttpPost("tags/save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTag(Tag tag)
        {
            if (tag.Id == 0) { tag.Slug = PostService.GenerateSlug(tag.Name); _db.Tags.Add(tag); }
            else _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Tag saved.";
            return RedirectToAction(nameof(Tags));
        }

        // ---- Subscribers ----
        [HttpGet("subscribers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Subscribers()
        {
            var subs = await _db.NewsletterSubscribers
                .OrderByDescending(s => s.SubscribedAt).ToListAsync();
            return View(subs);
        }

        // ---- Users ----
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = _users.Users.ToList();
            return View(users);
        }

        private async Task<PostFormViewModel> BuildPostFormAsync(PostFormViewModel? existing)
        {
            var vm = existing ?? new PostFormViewModel();
            vm.Categories = await _db.Categories.OrderBy(c => c.SortOrder).ToListAsync();
            vm.AllTags = await _db.Tags.OrderBy(t => t.Name).ToListAsync();
            return vm;
        }
    }
}
