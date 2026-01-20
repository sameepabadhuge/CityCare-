using CityCare.Data;
using CityCare.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public NotificationController(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // ✅ NEW: API endpoint to get unread count
    [HttpGet]
    public async Task<IActionResult> GetUnreadCount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { count = 0 });

        var count = await _db.Notifications.CountAsync(n => n.UserId == user.Id && !n.IsRead);
        return Json(new { count });
    }

    // List notifications
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var list = await _db.Notifications
            .Where(n => n.UserId == user.Id)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();

        return View(list);
    }

    // Mark all as read
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var unread = await _db.Notifications
            .Where(n => n.UserId == user.Id && !n.IsRead)
            .ToListAsync();

        foreach (var n in unread) n.IsRead = true;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Mark one as read and go to Issue details (optional)
    public async Task<IActionResult> Open(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);
        if (n == null) return NotFound();

        n.IsRead = true;
        await _db.SaveChangesAsync();

        // redirect to issue details depending on role
        if (n.IssueId != null)
        {
            if (User.IsInRole("Staff"))
                return RedirectToAction("Details", "Staff", new { id = n.IssueId });
            else
                return RedirectToAction("Details", "Issue", new { id = n.IssueId });
        }

        return RedirectToAction(nameof(Index));
    }
}
