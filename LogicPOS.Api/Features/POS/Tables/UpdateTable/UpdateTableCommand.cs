using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Tables.UpdateTable
{
    public class UpdateTableCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid PlaceId { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
