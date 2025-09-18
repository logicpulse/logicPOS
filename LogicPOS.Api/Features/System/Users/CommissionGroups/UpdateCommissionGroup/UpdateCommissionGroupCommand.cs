using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.CommissionGroups.UpdateCommissionGroup
{
    public class UpdateCommissionGroupCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; } 
        public decimal NewCommission { get; set; } 
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
