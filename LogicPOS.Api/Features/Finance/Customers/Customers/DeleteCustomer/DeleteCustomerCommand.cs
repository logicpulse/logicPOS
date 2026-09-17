using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Customers.DeleteCustomer
{
    public class DeleteCustomerCommand : DeleteCommand
    {
        public DeleteCustomerCommand(Guid id) : base(id)
        {
        }
    }
}
