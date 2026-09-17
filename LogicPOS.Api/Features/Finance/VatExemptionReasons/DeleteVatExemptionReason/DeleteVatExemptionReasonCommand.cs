using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatExcemptionReasons.DeleteVatExcemptionReason
{
    public class DeleteVatExemptionReasonCommand : DeleteCommand
    {
        public DeleteVatExemptionReasonCommand(Guid id) : base(id)
        {
        }
    }
}
