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

        // ✅ Ensure DB + tables exist
        await db.Database.MigrateAsync();

        // ✅ Roles
        await EnsureRoleAsync(roleManager, "Citizen");
        await EnsureRoleAsync(roleManager, "Staff");

        // ✅ Cities
        if (!await db.Cities.AnyAsync())
        {
            db.Cities.AddRange(
                new City { Name = "Kandy", Code = "KDY" },
                new City { Name = "Badulla", Code = "BDL" }
            );
            await db.SaveChangesAsync();
        }

        // ✅ Departments
        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Name = "Water", Code = "WTR" },
                new Department { Name = "Garbage", Code = "GRB" }
            );
            await db.SaveChangesAsync();
        }

        // ✅ Staff Access Codes (manual structure)
        if (!await db.StaffAccessCodes.AnyAsync())
        {
            var kdy = await db.Cities.FirstAsync(x => x.Code == "KDY");
            var wtr = await db.Departments.FirstAsync(x => x.Code == "WTR");

            var code = $"CC-{kdy.Code}-{wtr.Code}-{DateTime.UtcNow.Year}";

            db.StaffAccessCodes.Add(new StaffAccessCode
            {
                Code = code,
                CityId = kdy.Id,
                DepartmentId = wtr.Id,
                Year = DateTime.UtcNow.Year,
                IsActive = true,
                StaffPhone = "+94XXXXXXXXX"
            });

            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create role '{roleName}': {errors}");
            }
        }
    }
}
