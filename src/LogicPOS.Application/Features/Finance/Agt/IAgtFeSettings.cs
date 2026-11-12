namespace LogicPOS.Application.Features.Finance.Agt;

public interface IAgtFeSettings
{
    /// <summary>
    /// Licence module + Angola + <c>SERVICE_AGT_FE_ENABLED</c>.
    /// Operational FE switch in the app (independent from having purchased the module).
    /// </summary>
    Task<bool> IsElectronicInvoicingEnabledAsync(CancellationToken ct = default);

    /// <summary>
    /// FE enabled + <c>SERVICE_AGT_SEND_DOCUMENTS</c>.
    /// </summary>
    Task<bool> IsAutoSendEnabledAsync(CancellationToken ct = default);

    /// <summary>
    /// FE enabled + <c>SERVICE_AGT_SYNC_VALIDATION_STATUS</c>.
    /// Background poll of AGT <c>obterEstado</c> for documents with <c>ValidationSyncPending</c>.
    /// </summary>
    Task<bool> IsValidationSyncEnabledAsync(CancellationToken ct = default);

    /// <summary>
    /// Minutes between automatic validation-status polls for the same document
    /// (<c>SERVICE_AGT_SYNC_VALIDATION_INTERVAL_MINUTES</c>, default 30).
    /// </summary>
    Task<int> GetValidationSyncIntervalMinutesAsync(CancellationToken ct = default);

    /// <summary>Raw preference <c>SERVICE_AGT_FE_ENABLED</c> (defaults to false when missing).</summary>
    Task<bool> IsFePreferenceEnabledAsync(CancellationToken ct = default);
}
