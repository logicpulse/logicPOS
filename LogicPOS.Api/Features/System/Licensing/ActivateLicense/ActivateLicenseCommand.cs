using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.ActivateLicense
{
    public class ActivateLicenseCommand  : IRequest<ErrorOr<ActivateLicenseResponse>>
    {
        public string Name { get; set; } 

        public string Company { get; set; } 

        public string FiscalNumber { get; set; } 

        public string Address { get; set; } 

        public string Email { get; set; }

        public string Phone { get; set; }

        public string HardwareId { get; set; }

        public string AssemblyVersion { get; set; } 

        public int IdCountry { get; set; } = 168;

        public string SoftwareKey { get; set; }
    }
}
