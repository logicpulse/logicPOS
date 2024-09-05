using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.VatExemptionReasons.AddVatExemptionReason
{
    public class AddVatExemptionReasonCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation {  get; set; }
        public string Acronym { get; set; }
        public string StandardApplicable { get; set; }
        public string Notes { get; set; }
    }
}
