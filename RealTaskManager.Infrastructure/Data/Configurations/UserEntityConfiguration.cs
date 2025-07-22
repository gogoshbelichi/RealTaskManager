using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);
        /*builder.HasIndex(u => u.IdentityId).IsUnique();
        builder.Property(u => u.IdentityId).IsRequired().HasMaxLength(36);*/
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256); // RFC 5321
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.Username).IsRequired().HasMaxLength(32);
        builder.Property(u => u.Roles);
    }
}