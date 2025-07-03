using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data;

public sealed class RealTaskManagerDbContext(DbContextOptions<RealTaskManagerDbContext> options)
    : DbContext(options)
{
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<TasksAssignedToUser> TasksAssignedToUsers => Set<TasksAssignedToUser>();
    public DbSet<TasksCreatedByUser> TasksCreatedByUsers => Set<TasksCreatedByUser>();
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RealTaskManagerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}