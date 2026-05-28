using AITrendsNews.Data;
using AITrendsNews.Services;
using AITrendsNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AITrendsNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _posts;
        private readonly ApplicationDbContext _db;

        public HomeController(IPostService posts, ApplicationDbContext db)
        {
            _posts = posts;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                FeaturedPosts = await _posts.GetFeaturedAsync(3),
                TrendingPosts = await _posts.GetTrendingAsync(5),
                LatestPosts = await _posts.GetLatestAsync(9),
                Categories = await _db.Categories.OrderBy(c => c.SortOrder).ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string email)
        {
            if (!string.IsNullOrWhiteSpace(email) &&
                !await _db.NewsletterSubscribers.AnyAsync(s => s.Email == email))
            {
                _db.NewsletterSubscribers.Add(new Models.NewsletterSubscriber { Email = email });
                await _db.SaveChangesAsync();
                TempData["SubscribeSuccess"] = "You have been subscribed!";
            }
            else
            {
                TempData["SubscribeError"] = "Already subscribed or invalid email.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error() => View();
    }
}
