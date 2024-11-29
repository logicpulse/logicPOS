using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.VatRates.DeleteVatRate
{
    public class DeleteVatRateCommand : DeleteCommand
    {
        public DeleteVatRateCommand(Guid id) : base(id)
        {
        }
    }
}
