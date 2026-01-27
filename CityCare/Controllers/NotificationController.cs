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

    // ✅ API endpoint to get unread count
    [HttpGet]
    public async Task<IActionResult> GetUnreadCount()
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return Json(new { count = 0 });

        var count = await _db.Notifications.CountAsync(n => n.UserId == uid && !n.IsRead);
        return Json(new { count });
    }

    // ✅ List notifications
    public async Task<IActionResult> Index()
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var list = await _db.Notifications
            .Where(n => n.UserId == uid)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();

        return View(list);
    }

    // ✅ Mark all as read
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var unread = await _db.Notifications
            .Where(n => n.UserId == uid && !n.IsRead)
            .ToListAsync();

        foreach (var n in unread)
            n.IsRead = true;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // ✅ Mark one as read + redirect
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(int id)
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);
        if (n == null) return NotFound();

        n.IsRead = true;
        await _db.SaveChangesAsync();

        if (n.IssueId != null)
        {
            if (User.IsInRole("Staff"))
                return RedirectToAction("Details", "Staff", new { id = n.IssueId });

            return RedirectToAction("Details", "Issue", new { id = n.IssueId });
        }

        return RedirectToAction(nameof(Index));
    }

    // ✅ Delete ONE notification
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var notif = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == uid);

        if (notif == null) return NotFound();

        _db.Notifications.Remove(notif);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ✅ Delete ALL read notifications
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearRead()
    {
        var uid = _userManager.GetUserId(User);   // ✅ FIXED HERE
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var readNotifs = await _db.Notifications
            .Where(n => n.UserId == uid && n.IsRead)
            .ToListAsync();

        if (readNotifs.Count > 0)
        {
            _db.Notifications.RemoveRange(readNotifs);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
