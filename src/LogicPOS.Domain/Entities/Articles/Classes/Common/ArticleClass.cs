using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articleclass")]
public class ArticleClass : Entity.WithCode.AndOrder.AndDesignation
{
    public string Acronym { get; set; } = null!;
    public bool WorkInStock { get; set; }
        
    public async Task<Result> UpdateAsync(UpdateArticleClassDto dto,
        IArticleClassRepository repository,
        CancellationToken cancellationToken = default)
    {
        var updater = new ArticleClassUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }


    public static async Task<Result<ArticleClass>> CreateAsync(CreateArticleClassDto dto,
        IArticleClassRepository repository,
        CancellationToken cancellationToken = default)
    {
        var creator = new ArticleClassCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }

}