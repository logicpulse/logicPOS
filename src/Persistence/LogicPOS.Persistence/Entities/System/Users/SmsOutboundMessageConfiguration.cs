using LogicPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogicPOS.Persistence.Configurations;

public class SmsOutboundMessageConfiguration : IEntityTypeConfiguration<SmsOutboundMessage>
{
    public void Configure(EntityTypeBuilder<SmsOutboundMessage> builder)
    {
        builder.ToTable("SmsOutboundMessages");
        builder.Property(x => x.Destination).IsRequired();
        builder.Property(x => x.Purpose).IsRequired();
        builder.HasIndex(x => x.UserId);
    }
}
