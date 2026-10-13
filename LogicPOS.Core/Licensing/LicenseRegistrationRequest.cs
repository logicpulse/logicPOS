namespace LogicPOS.Core.Licensing;

public sealed record LicenseRegistrationRequest(
    string Name,
    string Company,
    string FiscalNumber,
    string Address,
    string Email,
    string Phone,
    string SoftwareKey,
    int CountryId = 168);