using System;

using LogicPOS.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogicPOS.Persistence.Entities.Articles.Compositions;

public class ArticleCompositionConfiguration : IEntityTypeConfiguration<ArticleComposition>
{
    public void Configure(EntityTypeBuilder<ArticleComposition> builder)
    {
        builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Child)
               .WithMany()
               .HasForeignKey(x => x.ChildId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
