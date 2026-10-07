using System.ComponentModel.DataAnnotations;

using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articlesubfamily")]
public class ArticleSubfamily : Entity.WithCode.AndOrder.AndDesignation
{
    public ArticleFamily? Family { get; set; }
    public Guid FamilyId { get; set; }

    public CommissionGroup? CommissionGroup { get; set; }
    public Guid? CommissionGroupId { get; set; }

    public DiscountGroup? DiscountGroup { get; set; }
    public Guid? DiscountGroupId { get; set; }

    public VatRate? VatOnTable { get; set; }
    public Guid? VatOnTableId { get; set; }

    public VatRate? VatDirectSelling { get; set; }
    public Guid? VatDirectSellingId { get; set; }

    [Required]
    public Button? Button { get; set; }

    public async Task<Result> UpdateAsync(UpdateArticleSubfamilyDto dto,
                                          IArticleSubfamilyRepository repository,
                                          ArticleSubfamilyReferences dependencies,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new ArticleSubfamilyUpdater(this, dto, repository, dependencies);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<ArticleSubfamily>> CreateAsync(CreateArticleSubfamilyDto dto,
                                                                   IArticleSubfamilyRepository repository,
                                                                   ArticleSubfamilyReferences dependencies,
                                                                   CancellationToken cancellationToken = default)
    {
        var creator = new ArticleSubfamilyCreator(dto, repository, dependencies);
        return await creator.CreateAsync(cancellationToken);
    }

}
