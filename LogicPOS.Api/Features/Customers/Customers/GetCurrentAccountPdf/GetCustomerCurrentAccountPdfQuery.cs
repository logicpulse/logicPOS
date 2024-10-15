using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Customers.GetCurrentAccountPdf
{
    public class GetCustomerCurrentAccountPdfQuery : IRequest<ErrorOr<string>>
    {
        public Guid CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
