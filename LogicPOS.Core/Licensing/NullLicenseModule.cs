namespace LogicPOS.Core.Licensing;

public sealed class NullLicenseModule : ILicenseModule
{
    public bool RegistrationRequired => false;
}
