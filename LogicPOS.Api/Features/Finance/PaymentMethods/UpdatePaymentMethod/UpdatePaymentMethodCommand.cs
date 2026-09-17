using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PaymentMethods.UpdatePaymentMethod
{
    public class UpdatePaymentMethodCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Token { get; set; }
        public string ResourceString { get; set; }
        public string ButtonIcon { get; set; }
        public string Acronym { get; set; }
        public string AllowPayback { get; set; }
        public string Symbol { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
