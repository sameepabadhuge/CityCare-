using CityCare.Data;
using CityCare.Models.Entities;
using CityCare.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(AppDbContext db, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // --------------------------
    // LOGIN
    // --------------------------
    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            vm.Password,
            vm.RememberMe,
            lockoutOnFailure: true
        );

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        // ✅ Redirect based on user role
        return await RedirectAfterLogin(user);
    }

    // --------------------------
    // LOGOUT
    // --------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // --------------------------
    // CITIZEN REGISTER
    // --------------------------
    [HttpGet]
    public async Task<IActionResult> CitizenRegister()
    {
        ViewBag.Cities = await _db.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(new CitizenRegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CitizenRegister(CitizenRegisterViewModel vm)
    {
        ViewBag.Cities = await _db.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        if (!ModelState.IsValid) return View(vm);

        var cityExists = await _db.Cities.AnyAsync(c => c.Id == vm.CityId && c.IsActive);
        if (!cityExists)
        {
            ModelState.AddModelError(nameof(vm.CityId), "Please select a valid city.");
            return View(vm);
        }

        var user = new User
        {
            FullName = vm.FullName,
            UserName = vm.Email,
            Email = vm.Email,
            Address = vm.Address,
            CityId = vm.CityId,
            DepartmentId = null
        };

        var create = await _userManager.CreateAsync(user, vm.Password);
        if (!create.Succeeded)
        {
            foreach (var e in create.Errors) ModelState.AddModelError("", e.Description);
            return View(vm);
        }

        // ✅ Auto role (no UI role selection)
        await _userManager.AddToRoleAsync(user, "Citizen");

        // ✅ Redirect to Login after register
        return RedirectToAction(nameof(Login));
    }

    // --------------------------
    // STAFF REGISTER
    // --------------------------
    [HttpGet]
    public async Task<IActionResult> StaffRegister()
    {
        ViewBag.Cities = await _db.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Departments = await _db.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        return View(new StaffRegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StaffRegister(StaffRegisterViewModel vm)
    {
        ViewBag.Cities = await _db.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Departments = await _db.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        if (!ModelState.IsValid) return View(vm);

        var cityExists = await _db.Cities.AnyAsync(c => c.Id == vm.CityId && c.IsActive);
        var depExists = await _db.Departments.AnyAsync(d => d.Id == vm.DepartmentId && d.IsActive);

        if (!cityExists) ModelState.AddModelError(nameof(vm.CityId), "Please select a valid city.");
        if (!depExists) ModelState.AddModelError(nameof(vm.DepartmentId), "Please select a valid department.");
        if (!cityExists || !depExists) return View(vm);

        // ✅ Validate StaffAccessCode
        var codeRow = await _db.StaffAccessCodes.FirstOrDefaultAsync(x =>
            x.IsActive &&
            x.CityId == vm.CityId &&
            x.DepartmentId == vm.DepartmentId &&
            x.Code == vm.StaffAccessCode);

        if (codeRow == null)
        {
            ModelState.AddModelError(nameof(vm.StaffAccessCode),
                "Invalid staff access code for selected city and department.");
            return View(vm);
        }

        var user = new User
        {
            FullName = vm.FullName,
            UserName = vm.Email,
            Email = vm.Email,
            Address = null,
            CityId = vm.CityId,
            DepartmentId = vm.DepartmentId
        };

        var create = await _userManager.CreateAsync(user, vm.Password);
        if (!create.Succeeded)
        {
            foreach (var e in create.Errors) ModelState.AddModelError("", e.Description);
            return View(vm);
        }

        // ✅ Auto role (no UI role selection)
        await _userManager.AddToRoleAsync(user, "Staff");

        // ✅ Redirect to Login after register
        return RedirectToAction(nameof(Login));
    }

    // --------------------------
    // Helper: redirect by role
    // --------------------------
    private async Task<IActionResult> RedirectAfterLogin(User user)
    {
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Dashboard", "Admin");  // Changed from StaffCodes to Dashboard

        if (await _userManager.IsInRoleAsync(user, "Staff"))
            return RedirectToAction("Dashboard", "Staff");

        // default citizen
        return RedirectToAction("Dashboard", "Issue");
    }
}


