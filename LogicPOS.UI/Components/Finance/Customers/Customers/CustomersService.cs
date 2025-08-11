using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.GetAllCustomers;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Customers
{
    public static class CustomersService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;
        public static Customer DefaultCustomer => GetAllCustomers().FirstOrDefault(x => x.Name.ToLower() == GeneralUtils.GetResourceByName("global_final_consumer").ToLower());
        public static List<Customer> GetAllCustomers()
        {
            var customers = _mediator.Send(new GetAllCustomersQuery()).Result;

            if (customers.IsError != false)
            {
                ErrorHandlingService.HandleApiError(customers);
                return null;
            }

            return customers.Value.ToList();
        }

    }
}
