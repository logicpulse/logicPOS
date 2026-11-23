namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Pure close policy for <c>PaymentWindow</c>: after a successful issue the dialog
/// must always return true, even if the user Alt+F4s, Escapes, or the WM closes the window.
/// </summary>
public static class PaymentWindowCloseDecision
{
    /// <summary>Block Alt+F4 / system close while issuing and before a success result.</summary>
    public static bool ShouldCancelWhileIssuing(bool issuing, bool confirmPayment)
        => issuing && confirmPayment == false;

    /// <summary>
    /// After success (<paramref name="confirmPayment"/>), a non-Ok close must be turned into
    /// <c>Close(true)</c>. Skip when we are already closing with that result (avoids recursion).
    /// </summary>
    public static bool ShouldForceSuccessClose(bool confirmPayment, bool closingWithResult)
        => confirmPayment && closingWithResult == false;

    /// <summary>
    /// Escape on the success notice closes with true. Before success, Escape must be ignored
    /// (no Close(false), no accidental confirm).
    /// </summary>
    public static bool ShouldCloseNoticeOnEscape(bool confirmPayment, bool closingWithResult)
        => confirmPayment && closingWithResult == false;

    /// <summary>Escape before a successful issue must not close the window.</summary>
    public static bool ShouldIgnoreEscapeBeforeSuccess(bool confirmPayment)
        => confirmPayment == false;
}
