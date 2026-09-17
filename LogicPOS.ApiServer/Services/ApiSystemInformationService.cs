using LogicPOS.ApiServer.DTOs;
using Microsoft.Extensions.Options;

namespace LogicPOS.ApiServer.Services;

public sealed class ApiSystemInformationService
{
    private readonly SystemInformationResponse _systemInformation;

    public ApiSystemInformationService(IOptions<SystemInformationResponse> systemInformation)
    {
        _systemInformation = systemInformation.Value;
    }

    public SystemInformationResponse GetSystemInformation()
    {
        return new SystemInformationResponse
        {
            Culture = _systemInformation.Culture,
            CountryCode2 = _systemInformation.CountryCode2,
            Module = _systemInformation.Module
        };
    }
}
