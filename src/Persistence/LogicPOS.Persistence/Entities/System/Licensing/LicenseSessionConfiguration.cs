using LogicPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogicPOS.Persistence.Configurations;

public class LicenseSessionConfiguration : IEntityTypeConfiguration<LicenseSession>
{
    public void Configure(EntityTypeBuilder<LicenseSession> builder)
    {
        builder.ToTable("LicenseSessions");
        builder.Property(x => x.ClientKind).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.SessionKey).IsUnique();
        builder.HasIndex(x => x.LastHeartbeatAt);
        builder.HasIndex(x => x.UserId);
    }
}