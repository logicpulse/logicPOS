using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Currencies.AddCurrency
{
    public class AddCurrencyCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Acronym { get; set; }
        public string Symbol { get; set; } = "??";
        public string Entity { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Notes { get; set; }
    }
}
