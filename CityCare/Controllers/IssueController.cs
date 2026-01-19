using CityCare.Data;
using CityCare.Models.Entities;
using CityCare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Controllers;

[Authorize(Roles = "Citizen")]
public class IssueController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;

    public IssueController(AppDbContext db, UserManager<User> userManager, IWebHostEnvironment env)
    {
        _db = db;
        _userManager = userManager;
        _env = env;
    }

    // Citizen Dashboard (already)
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var issues = await _db.Issues
            .Include(i => i.City)
            .Where(i => i.CitizenId == user.Id)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return View(new CitizenDashboardViewModel { Issues = issues });
    }

    // -----------------------------
    // CREATE (GET)
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        await LoadCreateDropdownsAsync();

        // default city from citizen profile
        var vm = new CreateIssueViewModel
        {
            CityId = user.CityId ?? 0
        };

        return View(vm);
    }

    // -----------------------------
    // CREATE (POST)
    // -----------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateIssueViewModel vm)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        await LoadCreateDropdownsAsync();

        // if user didn't select, default to their city
        if (vm.CityId == 0 && user.CityId != null)
            vm.CityId = user.CityId.Value;

        if (!ModelState.IsValid)
            return View(vm);

        // validate city exists
        var cityOk = await _db.Cities.AnyAsync(c => c.Id == vm.CityId && c.IsActive);
        if (!cityOk)
        {
            ModelState.AddModelError(nameof(vm.CityId), "Please select a valid city.");
            return View(vm);
        }

        // create issue
        var issue = new Issue
        {
            Title = vm.Title,
            Description = vm.Description,
            Category = vm.Category,
            CityId = vm.CityId,
            LocationText = vm.LocationText,
            CitizenId = user.Id,
            Status = IssueStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Issues.Add(issue);
        await _db.SaveChangesAsync(); // creates Issue.Id

        // handle image upload (optional)
        if (vm.ImageFile != null && vm.ImageFile.Length > 0)
        {
            // basic image type check
            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(vm.ImageFile.ContentType))
            {
                ModelState.AddModelError(nameof(vm.ImageFile), "Please upload a JPG, PNG, or WEBP image.");
                return View(vm);
            }

            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "issues");
            Directory.CreateDirectory(uploadsRoot);

            var ext = Path.GetExtension(vm.ImageFile.FileName);
            var fileName = $"issue-{issue.Id}-{Guid.NewGuid():N}{ext}";
            var savePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await vm.ImageFile.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/issues/{fileName}";

            _db.IssueImages.Add(new IssueImage
            {
                IssueId = issue.Id,
                ImageUrl = imageUrl
            });

            await _db.SaveChangesAsync();
        }

        // create notification (citizen gets success message)
        _db.Notifications.Add(new Notification
        {
            UserId = user.Id,
            IssueId = issue.Id,
            Title = "Complaint Submitted",
            Message = "Your complaint was submitted successfully and is now Pending.",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Complaint submitted successfully!";
        return RedirectToAction(nameof(Dashboard));
    }

    private async Task LoadCreateDropdownsAsync()
    {
        ViewBag.Cities = await _db.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Categories = new List<SelectListItem>
        {
            new SelectListItem("Water", "Water"),
            new SelectListItem("Garbage", "Garbage")
        };
    }
}
