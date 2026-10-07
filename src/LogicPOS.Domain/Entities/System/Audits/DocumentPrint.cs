using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_systemprint")]
public class DocumentPrint : Entity
{
    public string? Designation { get; set; }
    public string? CopyNames { get; set; }
    public int PrintCopies { get; set; }
    public string? PrintMotive { get; set; }
    public bool SecondPrint { get; set; }
    public Guid? DocumentId { get; set; }
    public Guid? ReceiptId { get; set; }
    public bool IsThermalPrint { get; set; }
}