using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Tables.AddTable
{
    public class AddTableCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public Guid PlaceId { get; set; }
        public string Notes { get; set; }
    }
}
