using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Customers.DiscountGroups.AddDiscountGroup
{
    public class AddDiscountGroupCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Notes { get; set; }
    }
}
