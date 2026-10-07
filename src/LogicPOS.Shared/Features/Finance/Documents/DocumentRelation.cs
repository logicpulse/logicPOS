namespace LogicPOS.Shared.Features.Finance.Documents;

public record DocumentRelation {
    public Guid DocumentId { get; set; }
    public List<string>? RelatedDocuments { get; set; }
}