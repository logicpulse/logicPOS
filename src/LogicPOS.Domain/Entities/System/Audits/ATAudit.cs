using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Enums;


namespace LogicPOS.Domain.Entities;

//[Table("sys_systemauditat")]
public class ATAudit : Entity
{
    public DateTime Date { get; set; }
    public AtAuditType Type { get; set; }
    public string? PostData { get; set; }
    public int ReturnCode { get; set; }
    public string? ReturnMessage { get; set; }
    public string? ReturnRaw { get; set; }
    public string? DocumentNumber { get; set; }
    public string? ATDocCodeID { get; set; }
    public Document? Document { get; set; }
    public Guid? DocumentId { get; set; }
}
