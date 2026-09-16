namespace LogicPOS.ApiServer.DTOs;

public sealed class ArticleChildResponse
{
    public ArticleResponse? Article { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class ArticleChildRequest
{
    public Guid ArticleId { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class SetArticleChildrenRequest
{
    public List<ArticleChildRequest> Children { get; set; } = [];
}
