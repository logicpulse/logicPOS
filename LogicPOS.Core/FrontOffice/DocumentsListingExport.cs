namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Resolves Documents listing export cells by stable column keys (SortMemberPath),
/// not by translated header captions.
/// </summary>
public static class DocumentsListingExport
{
    public static string Cell(PosDocumentRow row, string columnKey) => columnKey switch
    {
        "DocumentDateText" => row.DocumentDateText,
        "Number" => row.Number,
        "Status" => row.Status,
        "EntityName" => row.EntityName,
        "TaxId" => row.TaxId,
        "FinalTotalText" => row.FinalTotalText,
        "PaidAmountText" => row.PaidAmountText,
        "DebitAmountText" => row.DebitAmountText,
        "AssociatedDocuments" => row.AssociatedDocuments,
        _ => string.Empty
    };
}
