using Microsoft.AspNetCore.Mvc;
using RootedWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace RootedWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly RootedContext _context;

        public UserController(RootedContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            var newUser = new User
            {
                Name = name,
                Email = email,
                Username = username,
                Password = password // 🔥 Save password directly (no hashing)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Log user in after registration
            HttpContext.Session.SetInt32("UserID", newUser.UserID);
            HttpContext.Session.SetString("Username", newUser.Username);

            return RedirectToAction("Index", "TaskItems");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            Console.WriteLine($"Typed password: {password}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                Console.WriteLine("❌ Invalid login attempt.");
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            Console.WriteLine("✅ Login success!");

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Username", user.Username!);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // You can keep HashPassword here if you want, but it is no longer used
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
