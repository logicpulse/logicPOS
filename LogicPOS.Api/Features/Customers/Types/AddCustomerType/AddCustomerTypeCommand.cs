using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LogicPOS.Api.Features.Customers.Types.AddCustomerType
{
    public class AddCustomerTypeCommand: IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
