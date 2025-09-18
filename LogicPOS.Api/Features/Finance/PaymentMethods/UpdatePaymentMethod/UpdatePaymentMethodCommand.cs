using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PaymentMethods.UpdatePaymentMethod
{
    public class UpdatePaymentMethodCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public string NewToken { get; set; }
        public string NewResourceString { get; set; }
        public string NewButtonIcon { get; set; }
        public string NewAcronym { get; set; }
        public string NewAllowPayback { get; set; }
        public string NewSymbol { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
