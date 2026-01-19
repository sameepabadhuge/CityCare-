using CityCare.Data;
using CityCare.Models.Entities;
using CityCare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Controllers;

[Authorize(Roles = "Citizen")]
public class IssueController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public IssueController(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Citizen Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var issues = await _db.Issues
            .Include(i => i.City)
            .Where(i => i.CitizenId == user.Id)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        var vm = new CitizenDashboardViewModel { Issues = issues };
        return View(vm);
    }

    // Placeholder for "Report New Issue" page (we build next)
    public IActionResult Create()
    {
        return View();
    }

    // Complaint details (we build next)
    public IActionResult Details(int id)
    {
        return View();
    }
}
