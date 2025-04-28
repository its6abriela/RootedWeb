using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RootedWeb.Models;

namespace RootedWeb.Controllers
{

    public class Badge
    {
        public string? Name { get; set; }
        public bool IsUnlocked { get; set; }
        public string? UnlockTip { get; set; }
    }


    public class DashboardController : Controller
    {
        private readonly RootedContext _context;

        public DashboardController(RootedContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Profile");
        }

        // Settings GET Page
        public IActionResult Settings()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        // Settings POST for changing password
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            if (user == null || user.Password != currentPassword)
            {
                ViewBag.ErrorMessage = "Current password is incorrect.";
                return View("Settings");
            }

            user.Password = newPassword;
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Password changed successfully!";
            return View("Settings");
        }

        //EDIT PROFILE
        [HttpPost]
        public IActionResult EditProfile(string newUsername, string newEmail)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            if (user != null)
            {
                user.Username = newUsername;
                user.Email = newEmail;
                _context.SaveChanges();
                ViewBag.SuccessMessage = "Profile updated successfully!";
            }

            return RedirectToAction("Profile");
        }

        // DELETING ACCOUNT SETTINGS
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Register", "User"); 
        }
        //Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);
            var tasksCompleted = _context.Tasks.Count(t => t.IsComplete && t.UserID == userId);
            var currentStreak = _context.Streaks.FirstOrDefault(s => s.UserID == userId);

            //BADGES

            List<Badge> badges = new List<Badge>();

            badges.Add(new Badge { Name = "🌱 Newbie", IsUnlocked = tasksCompleted >= 5, UnlockTip = "Complete 5 tasks to unlock!" });
            badges.Add(new Badge { Name = "🌿 Growing Strong", IsUnlocked = tasksCompleted >= 10, UnlockTip = "Complete 10 tasks to unlock!" });
            badges.Add(new Badge { Name = "🌳 Tree Hugger", IsUnlocked = tasksCompleted >= 20, UnlockTip = "Complete 20 tasks to unlock!" });
            badges.Add(new Badge { Name = "🔥 Streak Champion", IsUnlocked = currentStreak?.CurrentStreak >= 7, UnlockTip = "Maintain a 7-day streak to unlock!" });

            ViewBag.Badges = badges;
            //Quotes rotation
            var quotes = new List<string>
               {
                  "Believe you can and you're halfway there. 🌟",
                  "Success is the sum of small efforts repeated daily. 💪",
                  "Every day is a chance to get better. 🚀",
                  "Stay positive, work hard, make it happen. 🔥",
                  "Small steps every day lead to big changes. 🌱",
                  "Your future is created by what you do today, not tomorrow. 🌍",
                  "Push yourself, because no one else is going to do it for you. 🎯"
               };

            var random = new Random();
            var randomQuote = quotes[random.Next(quotes.Count)];

            ViewBag.Username = user?.Username ?? "Guest";
            ViewBag.TasksCompleted = tasksCompleted;
            ViewBag.CurrentStreak = currentStreak?.CurrentStreak ?? 0;
            ViewBag.MotivationalQuote = randomQuote;

            // tree API
            if (currentStreak?.CurrentStreak >= 7)
            {
                if (user != null)
                {
                    var treeService = new Services.TreePlantingService();
                    var result = await treeService.PlantTreeAsync(user.Username!);
                    ViewBag.TreePlantingMessage = result;
                }
                else 
                {
                    ViewBag.TreePlantingMessage = "❌ User not found. Cannot plant tree.";
                }

            }
            else
            {
                ViewBag.TreePlantingMessage = null;
            }


            return View(user);

        }

    }
}
