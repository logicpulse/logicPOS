using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Customers.DiscountGroups.UpdateDiscountGroup
{
    public class UpdateDiscountGroupCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
