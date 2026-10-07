using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using System.ComponentModel.DataAnnotations;
using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articlefamily")]
public class ArticleFamily : Entity.WithCode.AndOrder.AndDesignation
{
    public CommissionGroup? CommissionGroup { get; set; }
    public Guid? CommissionGroupId { get; set; }

    public DiscountGroup? DiscountGroup { get; set; }
    public Guid? DiscountGroupId { get; set; }

    [Required]
    public Button? Button { get; set; }

    public async Task<Result> UpdateAsync(UpdateArticleFamilyDto dto,
                                          IArticleFamilyRepository repository,
                                          ArticleFamilyReferences dependencies,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new ArticleFamilyUpdater(this, dto, repository, dependencies);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<ArticleFamily>> CreateAsync(CreateArticleFamilyDto dto,
                                                                IArticleFamilyRepository repository,
                                                                ArticleFamilyReferences dependencies,
                                                                CancellationToken cancellationToken = default)
    {
        var creator = new ArticleFamilyCreator(dto, repository, dependencies);
        return await creator.CreateAsync(cancellationToken);
    }

}
