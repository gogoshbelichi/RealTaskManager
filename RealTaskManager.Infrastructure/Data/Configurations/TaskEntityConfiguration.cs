using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data.Configurations;

public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.Status).IsRequired().HasMaxLength(50);
        builder.Property(t => t.CreatedAt).IsRequired();
        //builder.Property(t => t.CreatedBy).IsRequired();
        
        builder.Property(t => t.Status)
            .HasConversion(v => v.ToString(), v => Enum.Parse<TaskStatusEnum>(v));
        
        builder
            .HasOne(t => t.CreatedBy)
            .WithMany(u => u.TasksCreated)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        /*builder
            .HasMany(e => e.UsersAssigned)
            .WithMany(e => e.TasksAssignedTo)
            .UsingEntity<TasksAssignedToUser>(
                r => r
                    .HasOne<UserEntity>(e => e.User)
                    .WithMany(e => e.TasksAssignedToUser)
                    .HasForeignKey(e => e.UserId),
                l => l
                    .HasOne<TaskEntity>(e => e.Task)
                    .WithMany(e => e.TasksAssignedToUser)
                    .HasForeignKey(e => e.TaskId),
                joinEntity => joinEntity
                    .HasKey(tsd => new { tsd.TaskId, tsd.UserId }));*/
    }
}