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

    // Staff Dashboard with filter: all | pending | inprogress | resolved
    public async Task<IActionResult> Dashboard(string filter = "all")
    {
        var staff = await _userManager.GetUserAsync(User);
        if (staff == null) return RedirectToAction("Login", "Account");

        // staff must have city + department
        if (staff.CityId == null || staff.DepartmentId == null)
            return Content("Staff profile missing City/Department. Please contact admin.");

        var query = _db.Issues
            .Include(i => i.City)
            .Include(i => i.Citizen)
            .Where(i => i.CityId == staff.CityId); // beginner version: filter by city only

        // (Later we will also match department by category mapping or department table)
        // For now: city based filter is enough to start.

        query = filter.ToLower() switch
        {
            "pending" => query.Where(i => i.Status == IssueStatus.Pending),
            "inprogress" => query.Where(i => i.Status == IssueStatus.InProgress),
            "resolved" => query.Where(i => i.Status == IssueStatus.Resolved),
            _ => query
        };

        var issues = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();

        var vm = new StaffDashboardViewModel
        {
            Filter = filter,
            Issues = issues
        };

        return View(vm);
    }

    // Staff complaint details + status update (we build next)
    public IActionResult Details(int id)
    {
        return View();
    }
}
