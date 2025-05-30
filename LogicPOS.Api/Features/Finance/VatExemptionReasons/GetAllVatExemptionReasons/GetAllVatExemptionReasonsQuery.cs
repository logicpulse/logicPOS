using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.VatExemptionReasons.GetAllVatExemptionReasons
{ 
    public class GetAllVatExemptionReasonsQuery : IRequest<ErrorOr<IEnumerable<VatExemptionReason>>>
    {
    }
}
