using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Guia de transporte: circulação com código AT quando a empresa tem a obrigação marcada.
/// </summary>
public static class TransportGuideCirculationRules
{
    public static bool IsObligationMarked(string? preferenceValue)
        => string.Equals(preferenceValue?.Trim(), "True", StringComparison.OrdinalIgnoreCase)
            || string.Equals(preferenceValue?.Trim(), "1", StringComparison.Ordinal);

    /// <summary>Without the mark, printing does not wait for an AT document code.</summary>
    public static bool MayPrintWithoutAtCode(bool obligationMarked)
        => obligationMarked == false;

    public static bool HasAtDocumentCode(string? atDocCodeId)
        => string.IsNullOrWhiteSpace(atDocCodeId) == false;

    public static bool IsMovementStartAfterCommunication(DateTime? communicatedAt, DateTime? movementStart)
        => communicatedAt.HasValue
            && movementStart.HasValue
            && movementStart.Value > communicatedAt.Value;

    /// <summary>Refusal when goods may not leave with the guide; null when circulation is allowed.</summary>
    public static string? CirculationRefusal(
        bool obligationMarked,
        string? documentType,
        string? atDocCodeId,
        DateTime? movementStart,
        DateTime? communicatedAt,
        bool atCommunicationAvailable)
    {
        if (obligationMarked == false || PosDocumentRules.IsWayBill(documentType) == false)
        {
            return null;
        }

        if (atCommunicationAvailable == false && HasAtDocumentCode(atDocCodeId) == false)
        {
            return "Sem ligação à AT não é possível comunicar a guia. Os bens não podem sair.";
        }

        if (HasAtDocumentCode(atDocCodeId) == false)
        {
            return "A guia só circula depois de comunicada à AT e de receber o código.";
        }

        if (movementStart.HasValue
            && communicatedAt.HasValue
            && IsMovementStartAfterCommunication(communicatedAt, movementStart) == false)
        {
            return "A hora de início do transporte tem de ser posterior à comunicação à AT.";
        }

        return null;
    }

    public static string? IssueRefusalWhenOffline(
        bool obligationMarked,
        string? documentType,
        bool isDraft,
        bool atCommunicationAvailable)
    {
        if (isDraft
            || obligationMarked == false
            || PosDocumentRules.IsWayBill(documentType) == false
            || atCommunicationAvailable)
        {
            return null;
        }

        return "Sem ligação à AT não é possível comunicar a guia. Os bens não podem sair.";
    }
}