using CityCare.Data;
using CityCare.Models.DTOs;
using CityCare.Models.Entities;
using CityCare.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Controllers;

public class IssueController : Controller
{
    private readonly AppDbContext _db;

    public IssueController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Issue
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var issues = await _db.Issues
            .Include(i => i.User)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new IssueListVm
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category,
                Status = i.Status,
                CreatedAt = i.CreatedAt,
                UserName = i.User != null ? i.User.FullName : "Unknown"
            })
            .ToListAsync();

        return View(issues);
    }

    // GET: /Issue/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var issue = await _db.Issues
            .Include(i => i.User)
            .Include(i => i.Images)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null) return NotFound();

        return View(issue);
    }

    // GET: /Issue/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateIssueDto());
    }

    // POST: /Issue/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateIssueDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        // Check user exists
        var userExists = await _db.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
        {
            ModelState.AddModelError(nameof(dto.UserId), "User not found.");
            return View(dto);
        }

        var issue = new Issue
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            LocationText = dto.LocationText,
            Status = "Pending",
            UserId = dto.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Issues.Add(issue);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
