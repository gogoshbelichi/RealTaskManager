using Microsoft.EntityFrameworkCore;

namespace RealTaskManager.Infrastructure.Data;

public sealed class RealTaskManagerDbContext(DbContextOptions<RealTaskManagerDbContext> options)
    : DbContext(options)
{
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RealTaskManagerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}