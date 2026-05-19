using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.CommissionGroups.UpdateCommissionGroup
{
    public class UpdateCommissionGroupCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; } 
        public decimal Commission { get; set; } 
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
