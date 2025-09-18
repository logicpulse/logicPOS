using System.Collections.Generic;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseInformation
{
    public struct GetLicenseInformationResponse
    {
        public Dictionary<string, string> LicenseInformation { get; set; }
    }
}
