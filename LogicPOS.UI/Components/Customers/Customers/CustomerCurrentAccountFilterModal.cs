﻿using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.GetCurrentAccountPdf;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public class CustomerCurrentAccountFilterModal : Modal
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        #region Components
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private PageTextBox TxtCustomer { get; set; }
        private PageTextBox TxtStartDate { get; set; }
        private PageTextBox TxtEndDate { get; set; }
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();
        #endregion

        public CustomerCurrentAccountFilterModal(Window parent) : base(parent,
                                                                       GeneralUtils.GetResourceByName("report_customer_balance_summary"),
                                                                       new Size(500, 509),
                                                                       PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_date_picker.png")
        {
        }

        private void Initialize()
        {
            InitializeTxtCustomer();
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            AddEventsHandlers();
        }

        private void AddEventsHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            Validate();

            if (AllFieldsAreValid() == false)
            {
                return;
            }

            var result = _mediator.Send(CreateQuery()).Result;

            if (result.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this, result.FirstError);
                Run();
            }

            DocumentPrintingUtils.ShowPdf(this, result.Value);
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

        private void InitializeTxtEndDate()
        {
            TxtEndDate = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_date_end"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtEndDate.Entry.IsEditable = false;
            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
        }

        private void TxtEndDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtEndDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new PageTextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCustomer.Entry.IsEditable = false;

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;

            ValidatableFields.Add(TxtCustomer);
        }

        private void BtnSelectCustomer_Clicked(object sender, EventArgs e)
        {
            var page = new CustomersPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCustomer.Text = page.SelectedEntity.Name;
                TxtCustomer.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtStartDate()
        {
            TxtStartDate = new PageTextBox(this,
                                       GeneralUtils.GetResourceByName("global_date_start"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: true,
                                      includeKeyBoardButton: false);

            TxtStartDate.Entry.IsEditable = false;
            TxtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtStartDate.SelectEntityClicked += TxtStartDate_SelectEntityClicked;
        }

        private void TxtStartDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtStartDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var verticalLayout = new VBox(false, 0);

            verticalLayout.PackStart(TxtCustomer.Component, false, false, 0);
            verticalLayout.PackStart(TxtStartDate.Component, false, false, 0);
            verticalLayout.PackStart(TxtEndDate.Component, false, false, 0);

            return verticalLayout;
        }

        protected void Validate()
        {
            if (AllFieldsAreValid())
            {
                return;
            }

            Utilities.ShowValidationErrors(ValidatableFields);

            Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }
    }
}