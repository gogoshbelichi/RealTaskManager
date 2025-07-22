using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data;

public sealed class RealTaskManagerDbContext(DbContextOptions<RealTaskManagerDbContext> options)
    : IdentityDbContext<TaskManagerUser>(options) 
{
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<UserEntity> UserProfiles => Set<UserEntity>();
    public DbSet<TasksAssignedToUser> TasksAssignedToUsers => Set<TasksAssignedToUser>();
    //public DbSet<TasksCreatedByUser> TasksCreatedByUsers => Set<TasksCreatedByUser>();
    public DbSet<RefreshTokenData> RefreshTokens => Set<RefreshTokenData>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RealTaskManagerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}