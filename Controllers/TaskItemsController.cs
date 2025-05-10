using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RootedWeb.Models;

namespace RootedWeb.Controllers
{
    public class TaskItemsController : Controller
    {
        private readonly RootedContext _context;

        public TaskItemsController(RootedContext context)
        {
            _context = context;
        }

        // GET: TaskItems
        public async Task<IActionResult> Index()
        {
            //debug where looking for database
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userTasks = await _context.Tasks
               .Where(t => t.UserID == userId && !t.IsComplete)
                .ToListAsync();

            return View(userTasks);


        }

        // GET: TaskItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // GET: TaskItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: TaskItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Mood,IsComplete")] TaskItem taskItem)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            taskItem.UserID = userId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Task created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(taskItem);
        }


        // GET: TaskItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }
            return View(taskItem);
        }

        
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: TaskItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskID,Title,Mood,IsComplete,UserID")] TaskItem taskItem)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (id != taskItem.TaskID)
            {
                return NotFound();
            }

            // task is linked to the correct user
            taskItem.UserID = userId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Task updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tasks.Any(e => e.TaskID == taskItem.TaskID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(taskItem);
        }



        // GET: TaskItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // POST: TaskItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem != null)
            {
                _context.Tasks.Remove(taskItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //marks task as complete in ASP.NET MVC
        private bool TaskItemExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskID == id);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.IsComplete = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Garden");
        }

        public async Task<IActionResult> Completed()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var completedTasks = await _context.Tasks
                .Where(t => t.IsComplete && t.UserID == userId)
                .ToListAsync();

            return View(completedTasks);
        }


        [HttpPost]
        public async Task<IActionResult> UndoComplete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.IsComplete = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Completed));
        }

    }

}
