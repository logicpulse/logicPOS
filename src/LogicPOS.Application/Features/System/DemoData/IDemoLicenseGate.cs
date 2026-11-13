namespace LogicPOS.Application.Features.System.DemoData;

/// <summary>
/// Demo licence unlock (local <c>logicpos_Demo</c> and/or cloud Accounts.Demo).
/// </summary>
public interface IDemoLicenseGate
{
    bool IsDemo { get; }
}
