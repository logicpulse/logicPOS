using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.CommissionGroups.AddCommissionGroup
{
    public class AddCommissionGroupCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public decimal Commission { get; set; }
        public string Notes { get; set; }
    }
}
