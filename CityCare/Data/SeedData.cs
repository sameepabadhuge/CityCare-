using CityCare.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Data;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await db.Database.MigrateAsync();

        // -------------------------
        // Roles
        // -------------------------
        if (!await roleManager.RoleExistsAsync("Citizen"))
            await roleManager.CreateAsync(new IdentityRole("Citizen"));

        if (!await roleManager.RoleExistsAsync("Staff"))
            await roleManager.CreateAsync(new IdentityRole("Staff"));

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        // -------------------------
        // Default Admin User
        // -------------------------
        var adminEmail = "admin@citycare.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new User
            {
                FullName = "System Admin",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var created = await userManager.CreateAsync(admin, "Admin@123");
            if (created.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // -------------------------
        // Cities
        // -------------------------
        if (!await db.Cities.AnyAsync())
        {
            db.Cities.AddRange(
                new City { Name = "Kandy", Code = "KDY", IsActive = true },
                new City { Name = "Badulla", Code = "BDL", IsActive = true }
            );
            await db.SaveChangesAsync();
        }

        // -------------------------
        // Departments
        // -------------------------
        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Name = "Water", Code = "WTR", IsActive = true },
                new Department { Name = "Garbage", Code = "GRB", IsActive = true }
            );
            await db.SaveChangesAsync();
        }

        // -------------------------
        // Staff Access Codes (ALL combos)
        // -------------------------
        if (!await db.StaffAccessCodes.AnyAsync())
        {
            var year = DateTime.UtcNow.Year;
            var cities = await db.Cities.Where(c => c.IsActive).ToListAsync();
            var deps = await db.Departments.Where(d => d.IsActive).ToListAsync();

            foreach (var c in cities)
            {
                foreach (var d in deps)
                {
                    db.StaffAccessCodes.Add(new StaffAccessCode
                    {
                        Code = $"CC-{c.Code}-{d.Code}-{year}",
                        CityId = c.Id,
                        DepartmentId = d.Id,
                        Year = year,
                        IsActive = true
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
