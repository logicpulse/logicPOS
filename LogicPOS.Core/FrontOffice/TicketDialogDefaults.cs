namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Safe defaults for POS ticket confirm overlays: destructive Sim/Não must default to Não.
/// </summary>
public static class TicketDialogDefaults
{
    /// <summary>
    /// When the confirm caption is «Sim» and cancel is shown, Não/Cancel is the safe default
    /// (Enter/Escape must not confirm the destructive action).
    /// </summary>
    public static bool PreferCancelAsDefault(bool showCancel, string confirmText)
        => showCancel && string.Equals(confirmText, "Sim", StringComparison.Ordinal);
}
