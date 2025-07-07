using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.HasIndex(u => u.IdentityId).IsUnique();
        builder.Property(u => u.IdentityId).IsRequired().HasMaxLength(36);
    }
}