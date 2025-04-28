using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RootedWeb.Models;

namespace RootedWeb.Controllers
{
    public class GardenController : Controller
    {
        private readonly RootedContext _context;

        public GardenController(RootedContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? streakMessage = null)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var completedCount = await _context.Tasks
                .CountAsync(t => t.IsComplete && t.UserID == userId);

            //Streak Logic
            var today = DateTime.Today;
            var streak = await _context.Streaks.FirstOrDefaultAsync(s => s.UserID == userId);
            if (streak != null)
            {
                if (!DateTime.TryParse(streak.LastCompletionDate, out DateTime lastDate) || lastDate != today)
                {
                    var yesterday = today.AddDays(-1);
                    streak.CurrentStreak = (lastDate == yesterday) ? streak.CurrentStreak + 1 : 1;
                    streak.LastCompletionDate = today.ToString("yyyy-MM-dd");
                    await _context.SaveChangesAsync();
                    streakMessage = $"🎉 Streak updated! Day {streak.CurrentStreak} 🌟";
                }
            }
            //cycle through these emojis
            var emojis = new[] { "🌸", "🌻", "🌼", "🌷", "🌺", "🌳", "🌱" };
            var gardenElements = new List<string>();

            for (int i = 0; i < completedCount; i++)
            {
                gardenElements.Add(emojis[i % emojis.Length]);
            }

            ViewBag.Garden = gardenElements;
            ViewBag.StreakMessage = streakMessage;

            return View();
        }


    }
}
