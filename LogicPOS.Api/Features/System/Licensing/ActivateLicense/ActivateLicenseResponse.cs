namespace LogicPOS.Api.Features.System.Licensing.ActivateLicense
{
    public struct ActivateLicenseResponse
    {
        public bool Success { get; set; }
        public string LicenseData { get; set; }
    }
}
