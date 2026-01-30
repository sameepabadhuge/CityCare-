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
    private readonly IWebHostEnvironment _env;

    public IssueController(AppDbContext db, UserManager<User> userManager, IWebHostEnvironment env)
    {
        _db = db;
        _userManager = userManager;
        _env = env;
    }

    // -----------------------------
    // CITIZEN DASHBOARD
    // -----------------------------
    public async Task<IActionResult> Dashboard()
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var issues = await _db.Issues
            .Include(i => i.City)
            .Include(i => i.Department)
            .Where(i => i.CitizenId == uid)
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

        return View(new CreateIssueViewModel
        {
            CityId = user.CityId ?? 0
        });
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

        // IMPORTANT: reload dropdowns for re-rendering view on validation errors
        await LoadCreateDropdownsAsync();

        // Default city if not selected
        if (vm.CityId == 0 && user.CityId != null)
            vm.CityId = user.CityId.Value;

        if (!ModelState.IsValid)
            return View(vm);

        // Validate city exists
        var cityOk = await _db.Cities.AnyAsync(c => c.Id == vm.CityId && c.IsActive);
        if (!cityOk)
        {
            ModelState.AddModelError(nameof(vm.CityId), "Please select a valid city.");
            return View(vm);
        }

        // Validate department exists
        var deptOk = await _db.Departments.AnyAsync(d => d.Id == vm.DepartmentId && d.IsActive);
        if (!deptOk)
        {
            ModelState.AddModelError(nameof(vm.DepartmentId), "Please select a valid department.");
            return View(vm);
        }

        // ✅ Create issue (FIX: Category added)
        var issue = new Issue
        {
            Title = vm.Title?.Trim() ?? "",
            Description = vm.Description?.Trim() ?? "",
            Category = vm.Category?.Trim() ?? "",          // ✅ FIXED
            DepartmentId = vm.DepartmentId,                 // ✅ Department
            CityId = vm.CityId,
            LocationText = vm.LocationText?.Trim() ?? "",
            CitizenId = user.Id,
            Status = IssueStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Snapshot contact phone (optional)
        await FillContactPhoneAsync(issue);

        _db.Issues.Add(issue);
        await _db.SaveChangesAsync();

        // Upload image (optional)
        await SaveIssueImageIfExistsAsync(issue.Id, vm.ImageFile);

        // Citizen notification
        _db.Notifications.Add(new Notification
        {
            UserId = user.Id,
            IssueId = issue.Id,
            Title = "Complaint Submitted",
            Message = "Your complaint was submitted successfully and is now Pending.",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        });

        // Notify staff
        await NotifyStaffAsync(issue);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Complaint submitted successfully!";
        return RedirectToAction(nameof(Dashboard));
    }

    // -----------------------------
    // DETAILS (GET)
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var issue = await _db.Issues
            .Include(i => i.City)
            .Include(i => i.Department)
            .Include(i => i.Images)
            .Include(i => i.Rating)
            .FirstOrDefaultAsync(i => i.Id == id && i.CitizenId == uid);

        if (issue == null) return NotFound();

        var vm = new IssueDetailsViewModel
        {
            Issue = issue,
            CanRate = issue.Status == IssueStatus.Resolved && issue.Rating == null,
            ExistingStars = issue.Rating?.Stars,
            ExistingComment = issue.Rating?.Comment,
            RateVm = new RateIssueViewModel
            {
                IssueId = issue.Id,
                Stars = 5
            }
        };

        return View(vm);
    }

    // -----------------------------
    // RATE (POST)
    // -----------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rate(RateIssueViewModel vm)
    {
        var uid = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");

        var issue = await _db.Issues
            .Include(i => i.Rating)
            .FirstOrDefaultAsync(i => i.Id == vm.IssueId && i.CitizenId == uid);

        if (issue == null) return NotFound();

        if (issue.Status != IssueStatus.Resolved)
        {
            TempData["Error"] = "You can rate only after the complaint is resolved.";
            return RedirectToAction(nameof(Details), new { id = vm.IssueId });
        }

        if (issue.Rating != null)
        {
            TempData["Error"] = "You already rated this complaint.";
            return RedirectToAction(nameof(Details), new { id = vm.IssueId });
        }

        if (!ModelState.IsValid)
            return await Details(vm.IssueId);

        _db.Ratings.Add(new Rating
        {
            IssueId = issue.Id,
            Stars = vm.Stars,
            Comment = vm.Comment,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        await NotifyStaffRatingAsync(issue, vm.Stars);

        TempData["Success"] = "Thank you! Your rating was submitted.";
        return RedirectToAction(nameof(Details), new { id = vm.IssueId });
    }

    // -----------------------------
    // LOOKUP STAFF PHONE (GET)
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> LookupStaffPhone(int departmentId, int cityId)
    {
        if (departmentId <= 0 || cityId <= 0)
            return Json(new LookupStaffPhoneResponse { staffPhone = null });

        var deptOk = await _db.Departments
            .AsNoTracking()
            .AnyAsync(d => d.IsActive && d.Id == departmentId);

        if (!deptOk)
            return Json(new LookupStaffPhoneResponse { staffPhone = null });

        var staffRoleId = await _db.Roles
            .AsNoTracking()
            .Where(r => r.NormalizedName == "STAFF")
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(staffRoleId))
            return Json(new LookupStaffPhoneResponse { staffPhone = null });

        var phone = await _db.Users
            .AsNoTracking()
            .Where(u => u.CityId == cityId && u.DepartmentId == departmentId)
            .Join(_db.UserRoles.AsNoTracking().Where(ur => ur.RoleId == staffRoleId),
                u => u.Id,
                ur => ur.UserId,
                (u, ur) => u)
            .Where(u => !string.IsNullOrEmpty(u.PhoneNumber))
            .OrderBy(u => u.CreatedAt)
            .Select(u => u.PhoneNumber)
            .FirstOrDefaultAsync();

        return Json(new LookupStaffPhoneResponse
        {
            staffPhone = string.IsNullOrWhiteSpace(phone) ? null : phone
        });
    }

    // -----------------------------
    // Helpers
    // -----------------------------
    private async Task LoadCreateDropdownsAsync()
    {
        ViewBag.Cities = await _db.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Departments = await _db.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        // ✅ FIX: Categories list (prevents null reference in Create.cshtml)
        ViewBag.Categories = new List<string>
        {
            "Water",
            "Garbage",
            "Road",
            "Electricity",
            "Street Light",
            "Drainage",
            "Other"
        };
    }

    private async Task SaveIssueImageIfExistsAsync(int issueId, IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0) return;

        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(imageFile.ContentType))
            return;

        var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "issues");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(imageFile.FileName);
        var fileName = $"issue-{issueId}-{Guid.NewGuid():N}{ext}";
        var savePath = Path.Combine(uploadsRoot, fileName);

        using var stream = new FileStream(savePath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        var imageUrl = $"/uploads/issues/{fileName}";

        _db.IssueImages.Add(new IssueImage
        {
            IssueId = issueId,
            ImageUrl = imageUrl
        });

        await _db.SaveChangesAsync();
    }

    private async Task FillContactPhoneAsync(Issue issue)
    {
        var staffRoleId = await _db.Roles
            .AsNoTracking()
            .Where(r => r.NormalizedName == "STAFF")
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(staffRoleId)) return;

        var phone = await _db.Users
            .AsNoTracking()
            .Where(u => u.CityId == issue.CityId && u.DepartmentId == issue.DepartmentId)
            .Join(_db.UserRoles.AsNoTracking().Where(ur => ur.RoleId == staffRoleId),
                u => u.Id,
                ur => ur.UserId,
                (u, ur) => u)
            .Where(u => !string.IsNullOrEmpty(u.PhoneNumber))
            .OrderBy(u => u.CreatedAt)
            .Select(u => u.PhoneNumber)
            .FirstOrDefaultAsync();

        issue.ContactPhone = string.IsNullOrWhiteSpace(phone) ? null : phone;
    }

    private async Task NotifyStaffAsync(Issue issue)
    {
        var staffUsers = await _db.Users
            .Where(u => u.CityId == issue.CityId && u.DepartmentId == issue.DepartmentId)
            .ToListAsync();

        var deptName = await _db.Departments
            .Where(d => d.Id == issue.DepartmentId)
            .Select(d => d.Name)
            .FirstOrDefaultAsync();

        deptName ??= "Department";

        foreach (var staff in staffUsers)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = staff.Id,
                IssueId = issue.Id,
                Title = "New Complaint Assigned",
                Message = $"New {deptName} complaint: \"{issue.Title}\"",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });
        }
    }

    private async Task NotifyStaffRatingAsync(Issue issue, int stars)
    {
        var staffUsers = await _db.Users
            .Where(u => u.CityId == issue.CityId && u.DepartmentId == issue.DepartmentId)
            .ToListAsync();

        string starEmoji = stars switch
        {
            5 => "⭐⭐⭐⭐⭐",
            4 => "⭐⭐⭐⭐",
            3 => "⭐⭐⭐",
            2 => "⭐⭐",
            1 => "⭐",
            _ => ""
        };

        foreach (var staff in staffUsers)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = staff.Id,
                IssueId = issue.Id,
                Title = "Complaint Rated",
                Message = $"Complaint \"{issue.Title}\" received {starEmoji} rating.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });
        }

        await _db.SaveChangesAsync();
    }
}
