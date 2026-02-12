using System;
using System.Data;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseData
{
    public class LicenseData
    {
        public bool IsLicensed { get; set; }
        public string Version { get; set; }
        public string HardwareId { get; set; }
        public LicenseStatus Status { get; set; }
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Nif { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Reseller { get; set; }
        public bool StocksModule { get; set; }
        public bool AgtFeModule { get; set; }
        public DateTime? AllUpdateExpirationDate { get; set; }
        public int? AllNumberOfDevices { get; set; }
        public bool HasExpired { get; set; }
        public bool IsValid { get; set; }
    }
}