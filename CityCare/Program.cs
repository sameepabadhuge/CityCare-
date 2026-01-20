using CityCare.Data;
using CityCare.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (Login/Register uses email + password)
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    // Optional lockout (beginner friendly defaults)
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cookie paths (so unauth user goes to your login page)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed roles + cities + departments + staff access codes
await SeedData.SeedAsync(app.Services);

// ? Debug: Check if admin exists
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var admin = await userManager.FindByEmailAsync("admin@citycare.com");
    
    if (admin != null)
    {
        var roles = await userManager.GetRolesAsync(admin);
        Console.WriteLine("????????????????????????????????????????");
        Console.WriteLine($"? Admin user found!");
        Console.WriteLine($"   Email: {admin.Email}");
        Console.WriteLine($"   Name: {admin.FullName}");
        Console.WriteLine($"   Roles: {string.Join(", ", roles)}");
        Console.WriteLine("????????????????????????????????????????");
    }
    else
    {
        Console.WriteLine("????????????????????????????????????????");
        Console.WriteLine("? Admin user NOT found!");
        Console.WriteLine("   Please check your SeedData.cs");
        Console.WriteLine("????????????????????????????????????????");
    }
    
    // Also check roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
    Console.WriteLine($"Admin Role Exists: {adminRoleExists}");
}

app.Run();
