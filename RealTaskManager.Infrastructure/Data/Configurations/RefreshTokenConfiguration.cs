using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenData>
{
    public void Configure(EntityTypeBuilder<RefreshTokenData> modelBuilder)
    {
        modelBuilder.HasKey(t => t.Id);
        modelBuilder.HasIndex(t => t.Token);
        modelBuilder.HasOne(u => u.User)
            .WithMany(rt => rt.RefreshTokens);
    }
}