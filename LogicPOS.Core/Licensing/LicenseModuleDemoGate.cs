using LogicPOS.Application.Features.System.DemoData;

namespace LogicPOS.Core.Licensing;

/// <summary>Demo gate backed by <see cref="ILicenseModule.Demo"/>.</summary>
public sealed class LicenseModuleDemoGate : IDemoLicenseGate
{
    private readonly ILicenseModule _license;

    public LicenseModuleDemoGate(ILicenseModule license) => _license = license;

    public bool IsDemo => _license.Demo;
}
