using CityCare.Data;
using CityCare.Models.Entities;
using CityCare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    // ========================================
    // DASHBOARD (Home)
    // ========================================
    public async Task<IActionResult> Dashboard()
    {
        var stats = new AdminDashboardViewModel
        {
            TotalCities = await _db.Cities.CountAsync(),
            ActiveCities = await _db.Cities.CountAsync(c => c.IsActive),
            TotalDepartments = await _db.Departments.CountAsync(),
            ActiveDepartments = await _db.Departments.CountAsync(d => d.IsActive),
            TotalStaffCodes = await _db.StaffAccessCodes.CountAsync(),
            ActiveStaffCodes = await _db.StaffAccessCodes.CountAsync(s => s.IsActive)
        };

        return View(stats);
    }

    // ========================================
    // CITIES MANAGEMENT
    // ========================================
    public async Task<IActionResult> Cities()
    {
        var cities = await _db.Cities.OrderBy(c => c.Name).ToListAsync();
        return View(cities);
    }

    [HttpGet]
    public IActionResult CreateCity()
    {
        return View(new City());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCity(City city)
    {
        if (!ModelState.IsValid) return View(city);

        // Check duplicate code
        if (await _db.Cities.AnyAsync(c => c.Code == city.Code))
        {
            ModelState.AddModelError(nameof(city.Code), "City code already exists.");
            return View(city);
        }

        _db.Cities.Add(city);
        await _db.SaveChangesAsync();

        TempData["Success"] = "City created successfully!";
        return RedirectToAction(nameof(Cities));
    }

    [HttpGet]
    public async Task<IActionResult> EditCity(int id)
    {
        var city = await _db.Cities.FindAsync(id);
        if (city == null) return NotFound();
        return View(city);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCity(City city)
    {
        if (!ModelState.IsValid) return View(city);

        var existing = await _db.Cities.FindAsync(city.Id);
        if (existing == null) return NotFound();

        // Check duplicate code (excluding current)
        if (await _db.Cities.AnyAsync(c => c.Code == city.Code && c.Id != city.Id))
        {
            ModelState.AddModelError(nameof(city.Code), "City code already exists.");
            return View(city);
        }

        existing.Name = city.Name;
        existing.Code = city.Code;
        existing.IsActive = city.IsActive;

        await _db.SaveChangesAsync();

        TempData["Success"] = "City updated successfully!";
        return RedirectToAction(nameof(Cities));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCity(int id)
    {
        var city = await _db.Cities.FindAsync(id);
        if (city == null) return NotFound();

        city.IsActive = !city.IsActive;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Cities));
    }

    // ========================================
    // DEPARTMENTS MANAGEMENT
    // ========================================
    public async Task<IActionResult> Departments()
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        return View(departments);
    }

    [HttpGet]
    public IActionResult CreateDepartment()
    {
        return View(new Department());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDepartment(Department department)
    {
        if (!ModelState.IsValid) return View(department);

        // Check duplicate code
        if (await _db.Departments.AnyAsync(d => d.Code == department.Code))
        {
            ModelState.AddModelError(nameof(department.Code), "Department code already exists.");
            return View(department);
        }

        _db.Departments.Add(department);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Department created successfully!";
        return RedirectToAction(nameof(Departments));
    }

    [HttpGet]
    public async Task<IActionResult> EditDepartment(int id)
    {
        var department = await _db.Departments.FindAsync(id);
        if (department == null) return NotFound();
        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDepartment(Department department)
    {
        if (!ModelState.IsValid) return View(department);

        var existing = await _db.Departments.FindAsync(department.Id);
        if (existing == null) return NotFound();

        // Check duplicate code (excluding current)
        if (await _db.Departments.AnyAsync(d => d.Code == department.Code && d.Id != department.Id))
        {
            ModelState.AddModelError(nameof(department.Code), "Department code already exists.");
            return View(department);
        }

        existing.Name = department.Name;
        existing.Code = department.Code;
        existing.IsActive = department.IsActive;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Department updated successfully!";
        return RedirectToAction(nameof(Departments));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDepartment(int id)
    {
        var department = await _db.Departments.FindAsync(id);
        if (department == null) return NotFound();

        department.IsActive = !department.IsActive;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Departments));
    }

    // ========================================
    // STAFF ACCESS CODES MANAGEMENT
    // ========================================
    public async Task<IActionResult> StaffCodes()
    {
        var codes = await _db.StaffAccessCodes
            .Include(x => x.City)
            .Include(x => x.Department)
            .OrderBy(x => x.City.Name)
            .ThenBy(x => x.Department.Name)
            .ToListAsync();

        return View(codes);
    }

    [HttpGet]
    public async Task<IActionResult> CreateStaffCode()
    {
        ViewBag.Cities = await _db.Cities.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
        ViewBag.Departments = await _db.Departments.Where(d => d.IsActive).OrderBy(d => d.Name).ToListAsync();
        
        return View(new CreateStaffCodeViewModel { Year = DateTime.UtcNow.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStaffCode(CreateStaffCodeViewModel vm)
    {
        ViewBag.Cities = await _db.Cities.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
        ViewBag.Departments = await _db.Departments.Where(d => d.IsActive).OrderBy(d => d.Name).ToListAsync();

        if (!ModelState.IsValid) return View(vm);

        var city = await _db.Cities.FindAsync(vm.CityId);
        var department = await _db.Departments.FindAsync(vm.DepartmentId);

        if (city == null || department == null)
        {
            ModelState.AddModelError("", "Invalid city or department selected.");
            return View(vm);
        }

        // Check if code already exists
        var codeExists = await _db.StaffAccessCodes.AnyAsync(s =>
            s.CityId == vm.CityId &&
            s.DepartmentId == vm.DepartmentId &&
            s.Year == vm.Year);

        if (codeExists)
        {
            ModelState.AddModelError("", "A code already exists for this city/department/year combination.");
            return View(vm);
        }

        // Auto-generate code format: CC-{CityCode}-{DeptCode}-{Year}
        var code = $"CC-{city.Code}-{department.Code}-{vm.Year}";

        _db.StaffAccessCodes.Add(new StaffAccessCode
        {
            Code = code,
            CityId = vm.CityId,
            DepartmentId = vm.DepartmentId,
            Year = vm.Year,
            IsActive = true
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Staff access code created: {code}";
        return RedirectToAction(nameof(StaffCodes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCode(int id)
    {
        var code = await _db.StaffAccessCodes.FindAsync(id);
        if (code == null) return NotFound();

        code.IsActive = !code.IsActive;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(StaffCodes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCode(int id)
    {
        var code = await _db.StaffAccessCodes.FindAsync(id);
        if (code == null) return NotFound();

        _db.StaffAccessCodes.Remove(code);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Staff access code deleted.";
        return RedirectToAction(nameof(StaffCodes));
    }
}
