﻿using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DocumentDetailDto = LogicPOS.Api.Features.Documents.AddDocument.DocumentDetail;


namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal : Modal
    {
        private IEnumerable<PaymentMethod> _paymentMethods;
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        private PaymentMethod _selectedPaymentMethod;
        private PaymentCondition _selectedPaymentCondition;
        private decimal OrderTotalFinal { get; } = SaleContext.GetCurrentOrder().TotalFinal;
        private decimal TotalFinal { get; set; } = SaleContext.GetCurrentOrder().TotalFinal;
        private decimal TotalDelivery { get; set; }
        private decimal TotalChange { get; set; }

        public PaymentsModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_payments"),
                                                   new Size(633, 620),
                                                   PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_payments.png")
        {
            _paymentMethods = GetPaymentMethods();
            SelectDefaultPaymentMethod();
            UpdateLabels();
        }

        private void SelectDefaultPaymentMethod()
        {
            SelectPaymentMethodByToken("MONEY");
            BtnMoney.Sensitive = false;
            TotalDelivery = TotalFinal;
        }

        private void UpdateLabels()
        {
            LabelTotalValue.Text = TotalFinal.ToString();
            LabelDeliveryValue.Text = TotalDelivery.ToString();
            LabelChangeValue.Text = TotalChange.ToString();
        }

        private void InitializeLabels()
        {
            //Colors
            LabelTotal.ModifyFg(StateType.Normal, Color.FromArgb(101, 137, 171).ToGdkColor());
            LabelDelivery.ModifyFg(StateType.Normal, Color.FromArgb(101, 137, 171).ToGdkColor());
            LabelChange.ModifyFg(StateType.Normal, Color.FromArgb(101, 137, 171).ToGdkColor());
            LabelTotalValue.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
            LabelDeliveryValue.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
            LabelChangeValue.ModifyFg(StateType.Normal, Color.White.ToGdkColor());

            //Alignments
            LabelTotal.SetAlignment(0, 0.5F);
            LabelDelivery.SetAlignment(0, 0.5F);
            LabelChange.SetAlignment(0, 0.5F);
            LabelTotalValue.SetAlignment(1, 0.5F);
            LabelDeliveryValue.SetAlignment(1, 0.5F);
            LabelChangeValue.SetAlignment(1, 0.5F);

            //labels Font
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString("Bold 10");
            LabelTotal.ModifyFont(fontDescription);
            LabelDelivery.ModifyFont(fontDescription);
            LabelChange.ModifyFont(fontDescription);
            Pango.FontDescription fontDescriptionValue = Pango.FontDescription.FromString("Bold 12");
            LabelTotalValue.ModifyFont(fontDescriptionValue);
            LabelDeliveryValue.ModifyFont(fontDescriptionValue);
            LabelChangeValue.ModifyFont(fontDescriptionValue);
        }

        private void InitializeButtons()
        {
            InitializePaymentMethodButtons();

            PaymentMethodButtons = new List<IconButtonWithText> {
                BtnMoney,
                BtnCheck,
                BtnMB,
                BtnCreditCard,
                BtnDebitCard,
                BtnVisa,
                BtnCustomerCard,
                BtnCurrentAccount};

            AddEventHandlers();
        }

        private void EnableAllPaymentMethodButtons(bool enable = true)
        {
            foreach (var button in PaymentMethodButtons)
            {
                button.Sensitive = enable;
            }
        }

        private void AddEventHandlers()
        {
            BtnClearCustomer.Clicked += BtnClearCustomer_Clicked;
            BtnMoney.Clicked += BtnMoney_Clicked;
            BtnCheck.Clicked += BtnCheck_Clicked;
            BtnMB.Clicked += BtnMB_Clicked;
            BtnCreditCard.Clicked += BtnCreditCard_Clicked;
            BtnDebitCard.Clicked += BtnDebitCard_Clicked;
            BtnVisa.Clicked += BtnVisa_Clicked;
            BtnCustomerCard.Clicked += BtnCustomerCard_Clicked;
            BtnCurrentAccount.Clicked += BtnCurrentAccount_Clicked;
            PaymentMethodButtons.ForEach(button => { button.Clicked += BtnPaymentMethod_Clicked; });
            BtnInvoice.Clicked += BtnInvoice_Clicked;
            BtnNewCustomer.Clicked += BtnNewCustomer_Clicked;
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private bool SelectPaymentCondition()
        {
            var page = new PaymentConditionsPage(this, PageOptions.SelectionPageOptions);
            var selectPaymentConditionModal = new EntitySelectionModal<PaymentCondition>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentConditionModal.Run();
            var selectedPaymentCondition = page.SelectedEntity;
            selectPaymentConditionModal.Destroy();

            if (response == ResponseType.Ok && selectedPaymentCondition != null)
            {
                _selectedPaymentCondition = selectedPaymentCondition;
                return true;
            }

            return false;
        }

        private void SelectPaymentMethodByToken(string token)
        {
            _selectedPaymentMethod = _paymentMethods.FirstOrDefault(x => x.Token == token);
        }

        private void InitializePaymentMethodButtons()
        {
            BtnMoney = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_money"),
                                                             PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_money.png");

            BtnCheck = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_bank_check"),
                                                 PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_bank_check.png");

            BtnMB = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_cash_machine"),
                                              PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_cash_machine.png");

            BtnCreditCard = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_credit_card"),
                                                      PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_credit_card.png");

            BtnDebitCard = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_debit_card"),
                                                     PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_debit_card.png");

            BtnVisa = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_visa"),
                                                PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_visa.png");

            BtnCustomerCard = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_customer_card"),
                                                        PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_customer_card.png");

            BtnCurrentAccount = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_current_account"),
                                                          PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_current_account.png");
        }

        private void UpdateTotals()
        {
            if (!decimal.TryParse(TxtDiscount.Text, out decimal discount))
            {
                discount = 0;
            }

            var discountPrice = OrderTotalFinal * discount / 100;
            TotalFinal = OrderTotalFinal - discountPrice;

            if (_selectedPaymentMethod == null || _selectedPaymentMethod.Acronym != "NU")
            {
                TotalDelivery = TotalFinal;
                TotalChange = 0;
            }
            else
            {
                TotalChange = TotalDelivery - TotalFinal;
            }

            UpdateLabels();
        }

        public void ShowCustomerData(Customer customer)
        {
            TxtFiscalNumber.Text = customer.FiscalNumber;
            TxtCardNumber.Text = customer.CardNumber;
            TxtDiscount.Text = customer.Discount?.ToString();
            TxtAddress.Text = customer.Address;
            TxtLocality.Text = customer.Locality;
            TxtZipCode.Text = customer.ZipCode;
            TxtCity.Text = customer.City;
            TxtCountry.Text = customer.Country.Designation;
            TxtCountry.SelectedEntity = customer.Country;
            TxtNotes.Text = customer.Notes;
        }

        private IEnumerable<PaymentMethod> GetPaymentMethods()
        {
            var getResult = _mediator.Send(new GetAllPaymentMethodsQuery()).Result;

            if (getResult.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this, getResult.FirstError);
                return Enumerable.Empty<PaymentMethod>();
            }

            return getResult.Value;
        }

        private void Clear()
        {
            TxtCustomer.Clear();
            TxtFiscalNumber.Clear();
            TxtCardNumber.Clear();
            TxtDiscount.Clear();
            TxtDiscount.Text = "0";
            TxtAddress.Clear();
            TxtLocality.Clear();
            TxtZipCode.Clear();
            TxtCity.Clear();
            TxtCountry.Clear();
            TxtNotes.Clear();
        }

        protected void Validate()
        {
            if (AllFieldsAreValid())
            {
                return;
            }

            ValidationUtilities.ShowValidationErrors(ValidatableFields);
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

        private DocumentCustomer GetDocumentCustomer()
        {
            var country = TxtCountry.SelectedEntity as Country;

            return new DocumentCustomer
            {
                Name = TxtCustomer.Text,
                FiscalNumber = TxtFiscalNumber.Text,
                Address = TxtAddress.Text,
                Locality = TxtLocality.Text,
                ZipCode = TxtZipCode.Text,
                City = TxtCity.Text,
                Country = country.Designation,
                CountryId = country.Id
            };
        }

        private string GetDocumentType()
        {
            return BtnInvoice.Sensitive ? "FS" : "FT";
        }

        private IEnumerable<DocumentDetailDto> GetDocumentDetails()
        {
            return SaleContext.GetCurrentOrder().GetDocumentDetails();
        }

        private AddDocumentCommand CreateAddDocumentCommand()
        {
            var command = new AddDocumentCommand();

            command.Type = GetDocumentType();
            command.PaymentMethodId = _selectedPaymentMethod?.Id;
            command.PaymentConditionId = _selectedPaymentCondition?.Id;
            command.CustomerId = (TxtCustomer.SelectedEntity as Customer)?.Id;
            if (command.CustomerId == null)
            {
                command.Customer = GetDocumentCustomer();
            }
            command.Discount = decimal.Parse(TxtDiscount.Text);
            command.Details = GetDocumentDetails().ToList();

            return command;
        }
    }
}
