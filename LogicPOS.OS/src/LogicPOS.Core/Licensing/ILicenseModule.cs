namespace LogicPOS.Core.Licensing;

/// <summary>
/// Optional license port. The LogicPulse registration implementation stays outside this repository.
/// </summary>
public interface ILicenseModule
{
    bool RegistrationRequired { get; }
}
