using LogicPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogicPOS.Persistence.Entities.Finance.Countries.PostalLocalities;

public class PostalLocalityConfiguration : IEntityTypeConfiguration<PostalLocality>
{
    public void Configure(EntityTypeBuilder<PostalLocality> builder)
    {
        builder.ToTable("PostalLocalities");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ZipCode).HasMaxLength(32).IsRequired();
        builder.Property(item => item.Street).HasMaxLength(160);
        builder.Property(item => item.Locality).HasMaxLength(128).IsRequired();
        builder.Property(item => item.City).HasMaxLength(128).IsRequired();
        builder.HasIndex(item => new { item.CountryId, item.ZipCode });
        builder.HasOne(item => item.Country)
            .WithMany()
            .HasForeignKey(item => item.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
