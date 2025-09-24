namespace LogicPOS.Api.Features.System.GetSystemInformations
{
    public class SystemInformation
    {
        public string Culture { get; set; }
        public string CountryCode2 { get; set; }

        public bool IsPortugal => CountryCode2.ToLower() == "pt";
        public bool IsAngola => CountryCode2.ToLower() == "ao";

    }
}
