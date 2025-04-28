using Microsoft.AspNetCore.Mvc;
using RootedWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace RootedWeb.Controllers
{
    public class MoodCheckController : Controller
    {
        private readonly RootedContext _context;

        public MoodCheckController(RootedContext context)
        {
            _context = context;
        }


        //cycle through the dictionary of quotes
        private static readonly Dictionary<int, List<string>> EnergyQuotes = new()
        {
            { 1, new List<string> {
                "Just start. Small steps matter.",
                "Take it easy today. Be gentle with yourself.",
                "You don’t need to be perfect to begin."
            }},
            { 2, new List<string> {
                "A little movement is still progress.",
                "You're getting there. Keep going.",
                "Small wins are still wins."
            }},
            { 3, new List<string> {
                "You’ve got this!",
                "Focus on one thing and do it well.",
                "Momentum starts with one task."
            }},
            { 4, new List<string> {
                "You're on fire today!",
                "Use this energy to knock things out!",
                "Tackle something that matters."
            }},
            { 5, new List<string> {
                "Let’s gooo! Crush your goals!",
                "You’re unstoppable. What’s next?",
                "Bring the energy to your biggest task today!"
            }},
        };

        private static readonly Dictionary<int, int> LastQuoteIndex = new()
        {
            { 1, -1 }, { 2, -1 }, { 3, -1 }, { 4, -1 }, { 5, -1 }
        };

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Result(string mood, int energyLevel)
        {
            string quote = GetNextQuote(energyLevel);

            ViewBag.Mood = mood;
            ViewBag.EnergyLevel = energyLevel;
            ViewBag.Motivation = quote;

            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId != null)
            {
                var log = new MoodLog
                {
                    Mood = mood,
                    EnergyLevel = energyLevel,
                    EntryDate = DateTime.Today,
                    UserID = userId.Value
                };

                _context.MoodLogs.Add(log);
                await _context.SaveChangesAsync();
            }

            return View();
        }


        private string GetNextQuote(int energyLevel)
        {
            if (!EnergyQuotes.ContainsKey(energyLevel))
                return "You're doing your best — and that’s enough.";

            var quotes = EnergyQuotes[energyLevel];
            LastQuoteIndex[energyLevel] = (LastQuoteIndex[energyLevel] + 1) % quotes.Count;
            return quotes[LastQuoteIndex[energyLevel]];
        }

        public async Task<IActionResult> History(string range = "all")
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var query = _context.MoodLogs.Where(m => m.UserID == userId);

            DateTime today = DateTime.Today;

            switch (range.ToLower())
            {
                case "week":
                    query = query.Where(m => m.EntryDate >= today.AddDays(-7));
                    break;
                case "month":
                    query = query.Where(m => m.EntryDate.Month == today.Month && m.EntryDate.Year == today.Year);
                    break;
            }

            var logs = await query.OrderByDescending(m => m.EntryDate).ToListAsync();
            ViewBag.SelectedRange = range;

            return View(logs);
        }

    }
}
