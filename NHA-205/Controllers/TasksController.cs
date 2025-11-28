using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Codexly.Data;
using Codexly.Models;

namespace Codexly.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly Repository<Todo> _tasks;
        private readonly UserManager<User> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _tasks = new Repository<Todo>(context);
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var options = new QueryOptions<Todo>
            {
                Where = t => t.UserId.Equals(userId)
            };

            var all = await _tasks.GetAllAsync(options);
            return View(all);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Title)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                TempData["Error"] = "Task name is required";
                return RedirectToAction("Index");
            }

            var userId = _userManager.GetUserId(User);

            var item = new TaskItem
            {
                Title = Title,
                Description = "",
                IsDone = false,
                UserId = userId
            };

            await _tasks.AddAsync(item);

            TempData["Success"] = "Task added!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _tasks.GetByIdAsync(id, new QueryOptions<TaskItem>());
            if (item == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (item.UserId != userId) return Unauthorized();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskItem item)
        {
            var userId = _userManager.GetUserId(User);
            if (item.UserId != userId) return Unauthorized();

            if (ModelState.IsValid)
            {
                await _tasks.UpdateAsync(item);
                TempData["Success"] = "Task updated!";
                return RedirectToAction("Index");
            }

            return View(item);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _tasks.GetByIdAsync(id, new QueryOptions<TaskItem>());
            if (item == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (item.UserId != userId) return Unauthorized();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _tasks.GetByIdAsync(id, new QueryOptions<TaskItem>());
            if (item == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (item.UserId != userId) return Unauthorized();

            await _tasks.DeleteAsync(id);

            TempData["Success"] = "Task deleted!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDone(int id)
        {
            var item = await _tasks.GetByIdAsync(id, new QueryOptions<TaskItem>());
            if (item == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (item.UserId != userId) return Unauthorized();

            item.IsDone = !item.IsDone;
            await _tasks.UpdateAsync(item);

            return RedirectToAction("Index");
        }
    }
}
