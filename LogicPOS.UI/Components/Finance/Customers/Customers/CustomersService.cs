using LogicPOS.Api.Features.Articles.Articles.ExportArticlesToExcel;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Customers.GetAllCustomers;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Customers.Customers.ExportCustomersToExcel;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Customers
{
    public static class CustomersService
    {
        private static List<Customer> _customers;
        private static Customer _default;

        public static List<Customer> Customers
        {
            get
            {
                if (_customers == null)
                {
                    _customers = GetAllCustomers();
                }
                return _customers;
            }
        }

       public static List<AutoCompleteLine> AutocompleteLines => Customers.Select(c => new AutoCompleteLine
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        public static List<AutoCompleteLine> FiscalNumberAutocompleteLines => Customers.Select(c => new AutoCompleteLine
        {
            Id = c.Id,
            Name = c.FiscalNumber
        }).ToList();

        public static void RefreshCustomersCache()
        {
            _customers = GetAllCustomers();
        }

        public static Customer Default
        {
            get
            {
                if (_default == null)
                {
                    _default = Customers.FirstOrDefault(x => x.Name.ToLower() == GeneralUtils.GetResourceByName("global_final_consumer").ToLower());
                }

                return _default;
            }
        }

        private static List<Customer> GetAllCustomers()
        {
            var customers = DependencyInjection.Mediator.Send(new GetAllCustomersQuery()).Result;

            if (customers.IsError != false)
            {
                ErrorHandlingService.HandleApiError(customers);
                return null;
            }

            return customers.Value.ToList();
        }

        public static Customer GetById(Guid id)
        {
            return GetAllCustomers().Where(C => C.Id == id).FirstOrDefault();
        }

        public static string ExportCustomersToExcel()
        {
            var result = DependencyInjection.Mediator.Send(new ExportCustomersToExcelQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.Path;
        }
    }
}
