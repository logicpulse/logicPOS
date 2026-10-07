namespace LogicPOS.Application.Features.Finance.Documents.UnsettledDocuments;

/// <summary>
/// Post-filter for credit notes in the settlement list.
/// NC rows are only shown when the customer has lifetime credit to apply against other documents.
/// </summary>
public static class UnsettledCreditNoteVisibilityRules
{
    public static bool ShouldInclude(
        string documentType,
        Guid? parentId,
        bool parentPaid,
        decimal customerBalance) // lifetime balance: credit − debit (from FT/ND outstanding)
    {
        if (documentType != "NC")
        {
            return true;
        }

        if (customerBalance <= 0)
        {
            return false;
        }

        if (parentId is null || !parentPaid)
        {
            return false;
        }

        return true;
    }
}
