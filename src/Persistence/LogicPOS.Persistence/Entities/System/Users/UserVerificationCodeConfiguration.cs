using LogicPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogicPOS.Persistence.Configurations;

public class UserVerificationCodeConfiguration : IEntityTypeConfiguration<UserVerificationCode>
{
    public void Configure(EntityTypeBuilder<UserVerificationCode> builder)
    {
        builder.ToTable("UserVerificationCodes");
        builder.Property(x => x.CodeHash).IsRequired();
        builder.HasIndex(x => x.UserId);
    }
}
