using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data.Configurations;

/*public class TasksCreatedByUserConfiguration : IEntityTypeConfiguration<TasksCreatedByUser>
{
    public void Configure(EntityTypeBuilder<TasksCreatedByUser> builder)
    {
        // Many-to-many: Task <-> User
        builder
            .HasKey(ss => new { ss.TaskId, ss.UserId });
    }
}*/