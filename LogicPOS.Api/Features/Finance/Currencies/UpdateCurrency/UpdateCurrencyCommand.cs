using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Currencies.UpdateCurrency
{
    public class UpdateCurrencyCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; } 
        public string Acronym { get; set; } 
        public string Symbol { get; set; } 
        public string Entity { get; set; }
        public decimal ExchangeRate { get; set; } 
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
