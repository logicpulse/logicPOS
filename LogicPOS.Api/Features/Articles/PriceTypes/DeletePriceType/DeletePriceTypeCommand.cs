using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.PriceTypes.DeletePriceType
{
    public class DeletePriceTypeCommand : DeleteCommand
    {
        public DeletePriceTypeCommand(Guid id) : base(id)
        {
        }
    }
}
