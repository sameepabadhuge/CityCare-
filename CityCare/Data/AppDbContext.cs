using CityCare.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityCare.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<IssueImage> IssueImages => Set<IssueImage>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<StaffAccessCode> StaffAccessCodes => Set<StaffAccessCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // User -> Issues (1 to many)
        modelBuilder.Entity<Issue>()
            .HasOne(i => i.User)
            .WithMany(u => u.Issues)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Issue -> Images (1 to many)
        modelBuilder.Entity<IssueImage>()
            .HasOne(ii => ii.Issue)
            .WithMany(i => i.Images)
            .HasForeignKey(ii => ii.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification -> Issue (optional)
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Issue)
            .WithMany(i => i.Notifications)
            .HasForeignKey(n => n.IssueId)
            .OnDelete(DeleteBehavior.SetNull);

        // StaffAccessCode -> UsedByUser (optional)
        modelBuilder.Entity<StaffAccessCode>()
            .HasOne(s => s.UsedByUser)
            .WithMany()
            .HasForeignKey(s => s.UsedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
