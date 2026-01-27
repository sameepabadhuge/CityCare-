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
    // STAFF DASHBOARD
    // -----------------------------
    public async Task<IActionResult> Dashboard(string filter = "all")
    {
        var staff = await _userManager.GetUserAsync(User);
        if (staff == null) return RedirectToAction("Login", "Account");

        // ✅ Must have City + Department
        if (staff.CityId == null || staff.DepartmentId == null)
        {
            TempData["Error"] = "Your staff account is missing City or Department assignment. Contact admin.";
            return View(new StaffDashboardViewModel { Filter = filter, Issues = new List<Issue>() });
        }

        // ✅ Get staff department name (Water/Garbage) to match Issue.Category
        var dept = await _db.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == staff.DepartmentId && d.IsActive);

        if (dept == null)
        {
            TempData["Error"] = "Your department is not active. Contact admin.";
            return View(new StaffDashboardViewModel { Filter = filter, Issues = new List<Issue>() });
        }

        var query = _db.Issues
            .Include(i => i.City)
            .Include(i => i.Citizen)
            .Where(i =>
                i.CityId == staff.CityId &&
                i.Category.ToLower() == dept.Name.ToLower()); // ✅ Water staff sees Water only

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

        if (staff.CityId == null || staff.DepartmentId == null)
            return Unauthorized();

        var dept = await _db.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == staff.DepartmentId && d.IsActive);

        if (dept == null) return Unauthorized();

        var issue = await _db.Issues
            .Include(i => i.City)
            .Include(i => i.Citizen)
            .Include(i => i.Images)
            .FirstOrDefaultAsync(i =>
                i.Id == id &&
                i.CityId == staff.CityId &&
                i.Category.ToLower() == dept.Name.ToLower()); // ✅ protect by dept

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

        if (staff.CityId == null || staff.DepartmentId == null)
            return Unauthorized();

        var dept = await _db.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == staff.DepartmentId && d.IsActive);

        if (dept == null) return Unauthorized();

        var issue = await _db.Issues
            .Include(i => i.Citizen)
            .FirstOrDefaultAsync(i =>
                i.Id == vm.Issue.Id &&
                i.CityId == staff.CityId &&
                i.Category.ToLower() == dept.Name.ToLower()); // ✅ protect by dept

        if (issue == null) return NotFound();

        // ✅ Assign staff + update status
        issue.AssignedStaffId = staff.Id;
        issue.Status = vm.NewStatus;

        // ✅ Notify citizen
        _db.Notifications.Add(new Notification
        {
            UserId = issue.CitizenId,
            IssueId = issue.Id,
            Title = "Complaint Status Updated",
            Message = $"Your complaint \"{issue.Title}\" status changed to {vm.NewStatus}.",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = "Complaint status updated successfully.";
        return RedirectToAction(nameof(Details), new { id = issue.Id });
    }
}
