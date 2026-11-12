namespace LogicPOS.Application.Features.Finance.Agt;

public static class AgtFePreferenceTokens
{
    public const string ElectronicInvoicingEnabled = "SERVICE_AGT_FE_ENABLED";
    public const string AutoSendDocuments = "SERVICE_AGT_SEND_DOCUMENTS";
    public const string SyncValidationStatus = "SERVICE_AGT_SYNC_VALIDATION_STATUS";
    public const string SyncValidationIntervalMinutes = "SERVICE_AGT_SYNC_VALIDATION_INTERVAL_MINUTES";

    public const int DefaultSyncValidationIntervalMinutes = 30;
}
