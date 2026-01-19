using CityCare.Data;
using CityCare.Models.Entities;
using CityCare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Controllers;

[Authorize(Roles = "Staff")]
public class StaffController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public StaffController(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // -----------------------------
    // STAFF DASHBOARD (already)
    // -----------------------------
    public async Task<IActionResult> Dashboard(string filter = "all")
    {
        var staff = await _userManager.GetUserAsync(User);
        if (staff == null) return RedirectToAction("Login", "Account");

        var query = _db.Issues
            .Include(i => i.City)
            .Include(i => i.Citizen)
            .Where(i => i.CityId == staff.CityId);

        query = filter switch
        {
            "pending" => query.Where(i => i.Status == IssueStatus.Pending),
            "inprogress" => query.Where(i => i.Status == IssueStatus.InProgress),
            "resolved" => query.Where(i => i.Status == IssueStatus.Resolved),
            _ => query
        };

        var issues = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();

        return View(new StaffDashboardViewModel
        {
            Filter = filter,
            Issues = issues
        });
    }

    // -----------------------------
    // DETAILS (GET)
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var staff = await _userManager.GetUserAsync(User);
        if (staff == null) return RedirectToAction("Login", "Account");

        var issue = await _db.Issues
            .Include(i => i.City)
            .Include(i => i.Citizen)
            .Include(i => i.Images)
            .FirstOrDefaultAsync(i =>
                i.Id == id &&
                i.CityId == staff.CityId);

        if (issue == null) return NotFound();

        return View(new StaffIssueDetailsViewModel
        {
            Issue = issue,
            NewStatus = issue.Status
        });
    }

    // -----------------------------
    // UPDATE STATUS (POST)
    // -----------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(StaffIssueDetailsViewModel vm)
    {
        var staff = await _userManager.GetUserAsync(User);
        if (staff == null) return RedirectToAction("Login", "Account");

        var issue = await _db.Issues
            .Include(i => i.Citizen)
            .FirstOrDefaultAsync(i =>
                i.Id == vm.Issue.Id &&
                i.CityId == staff.CityId);

        if (issue == null) return NotFound();

        // Update status
        issue.Status = vm.NewStatus;

        // Create notification for citizen
        _db.Notifications.Add(new Notification
        {
            UserId = issue.CitizenId,
            IssueId = issue.Id,
            Title = "Complaint Status Updated",
            Message = $"Your complaint \"{issue.Title}\" status changed to {vm.NewStatus}.",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = "Complaint status updated successfully.";
        return RedirectToAction(nameof(Details), new { id = issue.Id });
    }
}
