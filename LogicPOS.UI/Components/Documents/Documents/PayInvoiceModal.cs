using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using Patagames.Pdf.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public class PayInvoiceModal : Modal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
       
        public PageTextBox TxtPaymentMethod { get; private set; }
        public PageTextBox TxtCurrency { get; private set; }
        public PageTextBox TxtTotalPaid { get; private set; }
        public PageTextBox TxtDateTime { get; private set; }
        public PageTextBox TxtNotes { get; private set; }
        private List<IValidatableField> ValidatableFields { get; set; } = new List<IValidatableField>();

        public PayInvoiceModal(Window parent) : base(parent,
                                                     GeneralUtils.GetResourceByName("window_title_dialog_pay_invoices"),
                                                     new Size(480, 444),
                                                     PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_pay_invoice.png")
        {
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

            var body = new VBox(false, 2);
            body.PackStart(TxtPaymentMethod.Component, false, false, 0);
            body.PackStart(TxtCurrency.Component, false, false, 0);
            body.PackStart(TxtTotalPaid.Component, false, false, 0);
            body.PackStart(TxtDateTime.Component, false, false, 0);
            body.PackStart(TxtNotes.Component, false, false, 0);

            return body;
        }

        private void Initialize()
        {
            InitializeTxtPaymentMethod();
            InitializeTxtCurrency();
            InitializeTxtTotalPaid();
            InitializeTxtDateTime();
            InitializeTxtNotes();

            ValidatableFields.Add(TxtPaymentMethod);
            ValidatableFields.Add(TxtCurrency);
            ValidatableFields.Add(TxtTotalPaid);
            ValidatableFields.Add(TxtDateTime);

            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (AllFieldsAreValid() == false)
            {
                ShowValidationErrors();
                Run();
                return;
            }
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(this,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtDateTime()
        {
            TxtDateTime = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_date"),
                                          isRequired: true,
                                          isValidatable: true,
                                          regex: RegexUtils.RegexDateTime,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);

            TxtDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void InitializeTxtTotalPaid()
        {
            TxtTotalPaid = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_total_deliver"),
                                          isRequired: true,
                                          isValidatable: true,
                                          regex: RegularExpressions.Money,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);
        }

        private void InitializeTxtPaymentMethod()
        {
            TxtPaymentMethod = new PageTextBox(this,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
                TxtPaymentMethod.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtCurrency()
        {
            TxtCurrency = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_currency"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCurrency.Entry.IsEditable = false;

            TxtCurrency.SelectEntityClicked += BtnSelectCurrency_Clicked;
        }

        private void BtnSelectCurrency_Clicked(object sender, EventArgs e)
        {
            var page = new CurrenciesPage(null, PageOptions.SelectionPageOptions);
            var selectCurrencyModal = new EntitySelectionModal<Currency>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCurrencyModal.Run();
            selectCurrencyModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCurrency.Text = page.SelectedEntity.Designation;
                TxtCurrency.SelectedEntity = page.SelectedEntity;
            }
        }

        public bool AllFieldsAreValid() => ValidatableFields.All(tab => tab.IsValid());

        protected void ShowValidationErrors() => Utilities.ShowValidationErrors(ValidatableFields);

    }
}
