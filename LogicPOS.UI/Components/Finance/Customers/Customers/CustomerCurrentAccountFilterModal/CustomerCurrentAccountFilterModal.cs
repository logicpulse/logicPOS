using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.GetCurrentAccountPdf;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerCurrentAccountFilterModal : Modal
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;

        private CustomerCurrentAccountFilterModal(Window parent) : base(parent,
                                                                       GeneralUtils.GetResourceByName("report_customer_balance_summary"),
                                                                       new Size(500, 509),
                                                                       AppSettings.Paths.Images + @"Icons\Windows\icon_window_date_picker.png")
        {
        }

        private void Initialize()
        {
            InitializeTxtCustomer();
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            AddEventsHandlers();
        }

        private void TxtCustomer_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtCustomer.Text))
            {
                TxtCustomer.Clear();
            }
        }
        private void AddEventsHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private GetCustomerCurrentAccountPdfQuery CreateQuery()
        {
            return new GetCustomerCurrentAccountPdfQuery
            {
                CustomerId = (TxtCustomer.SelectedEntity as Customer).Id,
                StartDate = DateTime.ParseExact(TxtStartDate.Text,"yyyy-MM-dd",CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(TxtEndDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            };
        }
        private void SelectCustomer(Customer customer)
        {
            TxtCustomer.SelectedEntity = customer;
            TxtCustomer.Text = customer.Name;
        }
    }
}