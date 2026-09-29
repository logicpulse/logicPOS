using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Customers.Types.DeleteCustomerType
{
    public class DeleteCustomerTypeCommand : DeleteCommand
    {
        public DeleteCustomerTypeCommand(Guid id) : base(id)
        {
        }
    }
}
