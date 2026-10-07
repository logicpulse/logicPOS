namespace LogicPOS.Application.Features.System.Licensing;

/// <summary>
/// License flags read from the cloud Accounts row for the current client.
/// A null reader result means local mode (no overlay). An inactive snapshot forces every AND to false.
/// </summary>
public sealed record CloudAccountLicense(
    bool IsActive,
    bool ModuleStocks,
    bool ModuleOnlineInvoicing,
    bool ModuleSms)
{
    public static CloudAccountLicense Inactive { get; } = new(false, false, false, false);
}
