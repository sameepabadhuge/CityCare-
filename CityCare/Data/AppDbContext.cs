using CityCare.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<City> Cities => Set<City>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<StaffAccessCode> StaffAccessCodes => Set<StaffAccessCode>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<IssueImage> IssueImages => Set<IssueImage>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<City>().HasIndex(x => x.Code).IsUnique();
        builder.Entity<Department>().HasIndex(x => x.Code).IsUnique();
        builder.Entity<StaffAccessCode>().HasIndex(x => x.Code).IsUnique();

        // StaffAccessCode -> City/Department
        builder.Entity<StaffAccessCode>()
            .HasOne(x => x.City).WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StaffAccessCode>()
            .HasOne(x => x.Department).WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> City/Department (nullable)
        builder.Entity<User>()
            .HasOne(x => x.City).WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<User>()
            .HasOne(x => x.Department).WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Issue relationships
        builder.Entity<Issue>()
            .HasOne(x => x.City).WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Issue>()
            .HasOne(x => x.Citizen).WithMany()
            .HasForeignKey(x => x.CitizenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Issue>()
            .HasOne(x => x.AssignedStaff).WithMany()
            .HasForeignKey(x => x.AssignedStaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<IssueImage>()
            .HasOne(x => x.Issue).WithMany(i => i.Images)
            .HasForeignKey(x => x.IssueId);

        builder.Entity<Notification>()
            .HasOne(x => x.Issue).WithMany(i => i.Notifications)
            .HasForeignKey(x => x.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Rating>()
            .HasOne(x => x.Issue).WithOne(i => i.Rating)
            .HasForeignKey<Rating>(x => x.IssueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
