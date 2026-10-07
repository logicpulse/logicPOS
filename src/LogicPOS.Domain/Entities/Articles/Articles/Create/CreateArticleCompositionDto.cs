namespace LogicPOS.Domain.Entities.Dtos;

public record CreateArticleCompositionDto (Guid ArticleId,
                                           Guid ArticleChildId,
                                           decimal Quantity);