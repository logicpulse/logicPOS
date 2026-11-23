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

    /// <summary>
    /// With a selection, export those rows; otherwise export every matching/filtered row.
    /// </summary>
    public static IReadOnlyList<PosDocumentRow> ResolveRows(
        IReadOnlyList<PosDocumentRow> selected,
        IReadOnlyList<PosDocumentRow> allMatching)
        => selected.Count > 0 ? selected : allMatching;

    /// <summary>Builds one export data row from stable column keys (never translated headers).</summary>
    public static IReadOnlyList<string> BuildRow(PosDocumentRow row, IReadOnlyList<string> columnKeys)
        => columnKeys.Select(key => Cell(row, key)).ToList();
}
