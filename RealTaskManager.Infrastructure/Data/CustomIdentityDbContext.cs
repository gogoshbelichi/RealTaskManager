using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data;

/*public class CustomIdentityDbContext(DbContextOptions<CustomIdentityDbContext> options)
    : IdentityDbContext<TaskManagerUser>(options)
{
    public DbSet<RefreshTokenData> RefreshTokens => Set<RefreshTokenData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshTokenData>().HasKey(t => t.Id);
        modelBuilder.Entity<RefreshTokenData>().HasIndex(t => t.Token);
        modelBuilder.Entity<RefreshTokenData>()
            .HasOne(u => u.User)
            .WithMany(rt => rt.RefreshTokens);
    }
}*/