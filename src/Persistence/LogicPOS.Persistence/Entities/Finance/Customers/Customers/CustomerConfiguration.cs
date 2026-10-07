using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogicPOS.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    { 
        builder.Property(x => x.Name).HasConversion<EncryptionService>();
        builder.Property(x => x.Address).HasConversion<EncryptionService>();
        builder.Property(x => x.Locality).HasConversion<EncryptionService>();
        builder.Property(x => x.ZipCode).HasConversion<EncryptionService>();
        builder.Property(x => x.City).HasConversion<EncryptionService>();
        builder.Property(x => x.FiscalNumber).HasConversion<EncryptionService>();
    }
}