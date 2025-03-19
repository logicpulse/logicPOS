using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Countries.DeleteCountry
{
    public class DeleteCountryCommand : DeleteCommand
    {
        public DeleteCountryCommand(Guid id) : base(id)
        {
        }
    }
}
