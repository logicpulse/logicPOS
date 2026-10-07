namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftWorkingDocuments
{
    public int NumberOfEntries => Documents.Count;
    public decimal TotalCredit { get; set; }
    public decimal TotalDebit { get; set; }
    public List<SaftWorkDocument> Documents { get; set; } = null!;
    
}