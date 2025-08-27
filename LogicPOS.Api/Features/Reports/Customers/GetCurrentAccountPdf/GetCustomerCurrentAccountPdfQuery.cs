using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Customers.GetCurrentAccountPdf
{
    public class GetCustomerCurrentAccountPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public Guid CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
