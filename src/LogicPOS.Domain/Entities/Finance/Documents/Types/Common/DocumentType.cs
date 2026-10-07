using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Enums;


namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinancetype")]
public class DocumentType : Entity.WithCode.AndOrder.AndDesignation
{
    #region Properties
    public string Acronym { get; set; } = null!;
    public int? AcronymLastSerie { get; set; }
    public bool Credit { get; set; }
    public int? CreditDebit { get; set; }
    public int PrintCopies { get; set; }
    public bool PrintRequestMotive { get; set; }
    public bool PrintRequestConfirmation { get; set; }
    public bool PrintOpenDrawer { get; set; }
    public bool WayBill { get; set; }
    public bool WsAtDocument { get; set; }
    public bool SaftAuditFile { get; set; }
    public SaftDocumentType SaftDocumentType { get; set; }
    public ProcessArticleStockMode StockMode { get; set; }
    #endregion
}
