namespace LogicPOS.Application.Features.System.Licensing;

public enum LicenseStatus
{
    Unlicensed = 0,
    Licensed = 1,
    Blocked = 2
}

public static class LicenseVersions
{
    public const string Unlicensed= "LOGICPOS_UNLICENSED";
    public const string Blocked = "LOGICPOS_BLOCK";
    public const string Licensed = "LOGICPOS_LICENSED";
}

public static class LicenseStatusHelper
{
    public static LicenseStatus FromString(string status) => status switch
    {
        LicenseVersions.Unlicensed => LicenseStatus.Unlicensed,
        LicenseVersions.Licensed => LicenseStatus.Licensed,
        LicenseVersions.Blocked => LicenseStatus.Blocked,
        _ => LicenseStatus.Unlicensed
    };
}