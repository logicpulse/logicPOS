using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.PriceTypes.AddPriceType
{
    public class AddPriceTypeCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Notes { get; set; }
        public int EnumValue { get; set; } = 0;
    }
}
