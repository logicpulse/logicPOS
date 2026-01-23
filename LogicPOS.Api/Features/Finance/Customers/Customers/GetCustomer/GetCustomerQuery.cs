using ErrorOr;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.GetCustomer
{
    public class GetCustomerQuery : IRequest<ErrorOr<Customer>>
    {
        public Guid? Id { get; set; } = null;
        public string FiscalNumber { get; set; }

        public GetCustomerQuery(Guid id)
        {
            Id = id;
        }

        public GetCustomerQuery(string fiscalNumber)
        {
            FiscalNumber = fiscalNumber;
        }

        public string GetUrlQuery()
        {
            if (Id.HasValue)
            {
                return $"?Id={Id}";
            }
            return $"?FiscalNumber={FiscalNumber}";
        }
    }
}
