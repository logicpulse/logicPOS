namespace LogicPOS.Core.FrontOffice;

public sealed class PosDocumentRow
{
    public Guid Id { get; init; }

    public DateTime DocumentDate { get; init; }

    public string DocumentDateText => DocumentDate.ToString("dd/MM/yyyy HH:mm:ss");

    public string Number { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string EntityName { get; init; } = string.Empty;

    public string TaxId { get; init; } = string.Empty;

    public decimal FinalTotal { get; init; }

    public string FinalTotalText => FinalTotal.ToString("N2");

    public decimal PaidAmount { get; init; }

    public string PaidAmountText => PaidAmount.ToString("N2");

    public decimal DebitAmount { get; init; }

    public string DebitAmountText => DebitAmount.ToString("N2");

    public string AssociatedDocuments { get; init; } = string.Empty;

    public bool CanEdit => Status == "Rascunho";

    public bool CanView => CanEdit == false;

    public string EditActionText => CanEdit ? "Editar" : "Ver";

    public bool CanDelete => Status != "Anulado";
}
