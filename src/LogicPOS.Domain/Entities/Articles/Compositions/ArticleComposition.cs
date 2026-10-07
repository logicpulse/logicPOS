using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_articlecomposition")]
public class ArticleComposition : Entity
{
    public Article? Parent { get; set; }
    public Guid ParentId { get; set; }

    public Article? Child { get; set; }
    public Guid ChildId { get; set; }
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal Quantity { get; set; }

    public static Task<Result<ArticleComposition>> CreateAsync(IArticleCompositionRepository repository,
                                                               IArticleRepository articleRepository,
                                                               CreateArticleCompositionDto dto,
                                                               CancellationToken cancellationToken = default)
    {
        var creator = new ArticleCompositionCreator(dto, repository, articleRepository);
        return creator.CreateAsync(cancellationToken);
    }
}
