using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.SizeUnits.AddSizeUnit
{
    public class AddSizeUnitCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Notes { get; set; }
    }
}
