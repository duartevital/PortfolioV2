using Microsoft.EntityFrameworkCore;
using VitalPhotography.Api.Models;

namespace VitalPhotography.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Photo> Photos => Set<Photo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Photo>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Title).HasMaxLength(200).IsRequired();
            e.Property(p => p.Category).HasMaxLength(50).IsRequired();
            e.Property(p => p.Description).HasMaxLength(2000);
            e.Property(p => p.ThumbnailUrl).HasMaxLength(500);
            e.Property(p => p.DisplayUrl).HasMaxLength(500);
            e.HasIndex(p => p.Order);
            e.HasIndex(p => p.Category);
        });
    }
}
