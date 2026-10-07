using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articletype")]
public class ArticleType : Entity.WithCode.AndOrder.AndDesignation
{
    public bool HasPrice { get; set; }

    public async Task<Result> UpdateAsync(UpdateArticleTypeDto dto,
                                          IArticleTypeRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new ArticleTypeUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<ArticleType>> CreateAsync(CreateArticleTypeDto dto,
                                                              IArticleTypeRepository repository,
                                                              CancellationToken cancellationToken = default)
    {
        var creator = new ArticleTypeCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
