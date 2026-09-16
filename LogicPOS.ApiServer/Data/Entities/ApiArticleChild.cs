namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiArticleChild
{
    public Guid Id { get; set; }
    public Guid ParentArticleId { get; set; }
    public Guid ChildArticleId { get; set; }
    public decimal Quantity { get; set; }
}
