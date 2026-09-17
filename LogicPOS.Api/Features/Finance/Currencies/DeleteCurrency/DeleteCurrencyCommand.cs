using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Currencies.DeleteCurrency
{
    public class DeleteCurrencyCommand : DeleteCommand
    {
        public DeleteCurrencyCommand(Guid id) : base(id)
        {
        }
    }
}
