namespace LogicPOS.ApiServer.DTOs;

public sealed class SystemInformationResponse
{
    public string Culture { get; set; } = "pt-PT";
    public string CountryCode2 { get; set; } = "PT";
    public string Module { get; set; } = "default";
}

public sealed class LatestVersionResponse
{
    public string Version { get; set; } = "0.0.0";
}
