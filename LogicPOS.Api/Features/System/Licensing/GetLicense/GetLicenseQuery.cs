using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetLicense
{
    public class GetLicenseQuery : IRequest<ErrorOr<GetLicenseResponse>>
  
    {
        public string HardwareId { get; set; }
        public string Version { get; set; }
        public bool HaveLicence { get; set; }
    }
}
