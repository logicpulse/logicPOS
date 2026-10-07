using LogicPOS.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogicPOS.Persistence.Entities.Documents.PaymentMethods;

public class DocumentPaymentMethodConfiguration : IEntityTypeConfiguration<DocumentPaymentMethod>
{
    public void Configure(EntityTypeBuilder<DocumentPaymentMethod> builder)
    {
        builder.Navigation(x => x.PaymentMethod).AutoInclude();
    }
}