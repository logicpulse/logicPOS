namespace LogicPOS.Api.Features.System.GetSystemInformations
{
    public class SystemInformation
    {
        public string Culture { get; set; }
        /// <summary>Fiscal country of the API (single source for AT/AGT/SAF-T rules). Not the company address country.</summary>
        public string CountryCode2 { get; set; }
        /// <summary>API DatabaseSettings.Module (e.g. default, cafe, seafoodstore).</summary>
        public string Module { get; set; }

        public bool IsPortugal => IsCountry("PT");
        public bool IsAngola => IsCountry("AO");
        public bool IsMozambique => IsCountry("MZ");

        private bool IsCountry(string code2) =>
            string.Equals(CountryCode2?.Trim(), code2, global::System.StringComparison.OrdinalIgnoreCase);
    }
}
