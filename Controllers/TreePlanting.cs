using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RootedWeb.Models;

namespace RootedWeb.Controllers
{
    public class TreePlantingController : Controller
    {
        private readonly RootedContext _context;

        public TreePlantingController(RootedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var today = DateTime.Today;
            var past30 = today.AddDays(-29);

            var streakDays = await _context.Streaks
                .Where(s => s.UserID == userId && s.Date >= past30 && s.Date <= today)
                .Select(s => s.Date.Date)
                .Distinct()
                .ToListAsync();

            ViewBag.IsEligible = streakDays.Count >= 30;

            var planted = await _context.TreePlantings
                .Where(p => p.UserID == userId)
                .ToListAsync();

            return View(planted);
        }

        [HttpPost]
        public async Task<IActionResult> Plant(string region)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var alreadyPlanted = await _context.TreePlantings
                .AnyAsync(t => t.UserID == userId && t.DatePlanted.Date == DateTime.Today);

            if (alreadyPlanted)
            {
                TempData["Message"] = "You've already planted a tree today! 🌱";
                return RedirectToAction("Index");
            }

            _context.TreePlantings.Add(new TreePlanting
            {
                UserID = userId.Value,
                DatePlanted = DateTime.Today,
                Region = region
            });

            await _context.SaveChangesAsync();

            TempData["Message"] = "🎉 Your tree has been planted!";
            return RedirectToAction("Index");
        }
    }
}
