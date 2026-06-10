using LogicPOS.Api.Features.Articles.Articles.ExportArticlesToExcel;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Customers.GetAllCustomers;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Customers.Customers.ExportCustomersToExcel;
using LogicPOS.Api.Features.Finance.Customers.Customers.GetCustomer;
using LogicPOS.Api.Features.Finance.Customers.Customers.RechargeCard;
using LogicPOS.Globalization;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Customers
{
    public static class CustomersService
    {
        private static Customer _default;
        private static List<Customer> _customers;
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

public static List<AutoCompleteLine> AutocompleteLines => GetAllCustomers().Select(c => new AutoCompleteLine
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        public static List<AutoCompleteLine> SuppliersAutocompleteLines => GetAllCustomers().Where(x => x.Supplier).Select(c => new AutoCompleteLine
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        public static List<AutoCompleteLine> FiscalNumberAutocompleteLines => GetAllCustomers().Select(c => new AutoCompleteLine
        {
            Id = c.Id,
            Name = c.FiscalNumber
        }).ToList();

        public static List<AutoCompleteLine> CardNumberAutocompleteLines => GetAllCustomers().Where(c => c.CardNumber != null).Select(c => new AutoCompleteLine
        {
            Id = c.Id,
            Name = c.CardNumber
        }).ToList();

        public static Customer GetByFiscalNumber(string fiscalNumber)
        {
            GetCustomerQuery query = new GetCustomerQuery(fiscalNumber);
            var customerResult = DependencyInjection.Mediator.Send(query).Result;
            if (customerResult.IsError)
            {
                ErrorHandlingService.HandleApiError(customerResult);
                return null;
            }
            return customerResult.Value;
        }

        public static Customer Default
        {
            get
            {
                if (_default == null)
                {
                    _default = GetAllCustomers().FirstOrDefault(x => x.Name.ToLower() == LocalizedString.Instance["global_final_consumer"].ToLower());
                }

                return _default;
            }
        }

        public static List<Customer> GetAllCustomers()
        {
            var customers = DependencyInjection.Mediator.Send(new GetAllCustomersQuery()).Result;

            if (customers.IsError != false)
            {
                ErrorHandlingService.HandleApiError(customers);
                return null;
            }

            return customers.Value.Where(c => c.IsDeleted == false).ToList();
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

        public static bool RechargeCard(Guid customerId, decimal amount)
        {
            var command = new RechargeCardCommand(customerId, amount);
            var result = DependencyInjection.Mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            _customers = null;
            return true;
        }

        public static bool CanRechargeCard(Customer customer)
        {
            return customer != null
                && customer.IsDeleted == false
                && string.IsNullOrWhiteSpace(customer.CardNumber) == false;
        }

        public static bool CanPayWithCustomerCard(Customer customer)
        {
            return customer != null
                && customer.Id != Guid.Empty
                && customer.IsDeleted == false
                && customer.IsFinalConsumer == false
                && string.IsNullOrWhiteSpace(customer.CardNumber) == false;
        }

        public static bool HasSufficientCardBalance(Customer customer, decimal amount)
        {
            return customer != null
                && (customer.CardMode == CardMode.Credit || customer.CardCredit >= amount);
        }
    }
}
