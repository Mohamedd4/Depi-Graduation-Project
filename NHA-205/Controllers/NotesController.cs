using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Codexly.Models;

namespace Codexly.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly Repository<Note> _noteRepo;
        private readonly UserManager<User> _userManager;

        public NotesController(Repository<Note> noteRepo, UserManager<User> userManager)
        {
            _noteRepo = noteRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var options = new QueryOptions<Note>
            {
                Where = n => n.UserId == userId,
                OrderBy = n => n.UpdatedAt
            };

            var notes = await _noteRepo.GetAllByIdAsync(userId, nameof(Note.UserId), options);

            return View(notes.OrderByDescending(n => n.UpdatedAt));
        }

        [HttpPost]
        public async Task<IActionResult> Add()
        {
            var userId = _userManager.GetUserId(User);

            var note = new Note
            {
                Title = "Untitled",
                ContentMarkdown = "",
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };

            await _noteRepo.AddAsync(note);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string Title, string Body)
        {
            var userId = _userManager.GetUserId(User);

            var options = new QueryOptions<Note>();
            var note = await _noteRepo.GetByIdAsync(id, options);

            if (note == null || note.UserId != userId)
                return Unauthorized();

            note.Title = string.IsNullOrWhiteSpace(Title) ? note.Title : Title;
            note.ContentMarkdown = Body;
            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepo.UpdateAsync(note);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var options = new QueryOptions<Note>();
            var note = await _noteRepo.GetByIdAsync(id, options);

            if (note == null || note.UserId != userId)
                return Unauthorized();

            await _noteRepo.DeleteAsync(id);

            return RedirectToAction("Index");
        }
    }
}
