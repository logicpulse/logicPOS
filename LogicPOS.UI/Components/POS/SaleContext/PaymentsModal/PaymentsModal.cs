using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.Documents.Rules;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.POS.Enums;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using DocumentDetailDto = LogicPOS.Api.Features.Documents.AddDocument.DocumentDetail;


namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal : Modal
    {
        private PaymentCondition _selectedPaymentCondition;
        private string _documentType = GetDefaultDocumentType();
        private PaymentMode _paymentMode = PaymentMode.Full;
        private List<SaleItem> _partialPaymentItems = new List<SaleItem>();
        public PaymentMethod PaymentMethod => _selectedPaymentMethod;
        private PaymentMethod _selectedPaymentMethod;
        private decimal OrderTotalFinal { get; } = SaleContext.CurrentOrder.TotalFinal;
        private decimal TotalFinal { get; set; } = SaleContext.CurrentOrder.TotalFinal;
        private decimal TotalDelivery { get; set; }
        private decimal TotalChange { get; set; }
        public static int InitialSplittersNumber { get; set; } = 0;
        private int SplittersNumber;

        public PaymentsModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_payments"),
                                                   new Size(633, 620),
                                                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_payments.png")
        {
            UpdateLabels();
            SetDefaultCustomer();
        }

        private void SetDefaultCustomer()
        {
            if (CustomersService.Default != null)
            {
                SelectCustomer(CustomersService.Default);
            }
        }

        public void SplitAccount(int splittersNumber = 2)
        {
            SplitModeInitilizer(splittersNumber);
            UpdateLabels();
            UpdateTotals();
        }

        private void SplitModeInitilizer(int splittersNumber)
        {
            SplittersNumber = splittersNumber;
            if (InitialSplittersNumber == 0)
            {
                InitialSplittersNumber = SplittersNumber;
            }
            _paymentMode = PaymentMode.Splited;
            TxtCustomer.Component.Sensitive = false;
            TxtCardNumber.Entry.Sensitive = false;
            TxtAddress.Entry.Sensitive = false;
            TxtCity.Entry.Sensitive = false;
            TxtCountry.Entry.Sensitive = false;
            TxtFiscalNumber.Entry.Sensitive = false;
            TxtLocality.Entry.Sensitive = false;
            TxtZipCode.Entry.Sensitive = false;

        }

        private void UpdateLabels()
        {
            LabelTotalValue.Text = TotalFinal.ToString("C");
            LabelDeliveryValue.Text = TotalDelivery.ToString("C");
            LabelChangeValue.Text = TotalChange.ToString("C");
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

        private void UpdateTotals()
        {
            UpdateTotalFinal();

            if (PaymentMethod == null || PaymentMethod.Acronym != "NU")
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

        private void UpdateTotalFinal()
        {
            switch (_paymentMode)
            {
                case PaymentMode.Full:
                    TotalFinal = OrderTotalFinal;
                    break;
                case PaymentMode.Partial:
                    TotalFinal = _partialPaymentItems.Sum(x => x.TotalFinal);
                    break;
                case PaymentMode.Splited:
                    if (InitialSplittersNumber != 0 && InitialSplittersNumber == SplittersNumber)
                    {
                        TotalFinal = OrderTotalFinal / InitialSplittersNumber;
                    }
                    else
                    {
                        TotalFinal = OrderTotalFinal;
                    }
                    break;
            }

            ApplyGlobalDiscount();
        }

        private void ApplyGlobalDiscount()
        {
            if (!decimal.TryParse(TxtDiscount.Text, out decimal discount))
            {
                discount = 0;
            }

            var discountPrice = TotalFinal * discount / 100;
            TotalFinal = TotalFinal - discountPrice;
        }

        public void ShowCustomerData(Customer customer)
        {
            TxtFiscalNumber.Text = customer.FiscalNumber;
            TxtCardNumber.Text = customer.CardNumber;
            TxtDiscount.Text = customer.Discount.ToString();
            TxtAddress.Text = customer.Address;
            TxtLocality.Text = customer.Locality;
            TxtZipCode.Text = customer.ZipCode;
            TxtCity.Text = customer.City;
            TxtCountry.Text = customer.Country.Designation;
            TxtCountry.SelectedEntity = customer.Country;
            TxtNotes.Text = customer.Notes;
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
            TxtCountry.Text = CountriesService.Default.Designation;
            TxtCountry.SelectedEntity = CountriesService.Default;
            TxtNotes.Clear();
            FreezeEditableFields(false);
        }

        protected bool Validate()
        {
            if (AllFieldsAreValid() == false)
            {
                ValidationUtilities.ShowValidationErrors(ValidatableFields);
                return false;
            }

            if (CompanyDetailsService.CompanyInformation.IsPortugal)
            {
                if (DocTypeAnalyzer.IsSimplifiedInvoice() && TotalFinal > DocumentRules.Portugal.SimplifiedInvoiceMaxTotal)
                {
                    string messageFormat = LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_max_value"];
                    string message = string.Format(messageFormat, $"Total: {TotalFinal:C}\nMáximo: {DocumentRules.Portugal.SimplifiedInvoiceMaxTotal:C}", LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_max_value_mode_paymentdialog"]);
                    
                    var response = CustomAlerts.Warning(this)
                        .WithButtonsType(ButtonsType.YesNo)
                        .WithMessage(message)
                        .ShowAlert();

                    if(response != ResponseType.Yes)
                    {
                        return false;
                    }

                    _documentType = "FR";
                }

                if (GetDocumentCustomer().FiscalNumber == CustomersService.Default.FiscalNumber && TotalFinal > DocumentRules.Portugal.FinalConsumerMaxTotal) {

                    string messageFormat = LocalizedString.Instance["dialog_message_value_exceed_simplified_invoice_for_final_or_annonymous_consumer"];
                    string message = string.Format(messageFormat, 
                        $"Total: {TotalFinal:C}",
                         $"Máximo: {DocumentRules.Portugal.FinalConsumerMaxTotal:C}");

                    var response = CustomAlerts.Warning(this)
                        .WithSize(new Size(550,480))
                        .WithMessage(message)
                        .ShowAlert();

                    return false;
                }

            }

            return true;
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
                Country = country.Code2,
                CountryId = country.Id
            };
        }

        private string GetDocumentType()
        {
            return (BtnInvoice.Sensitive == true) ? _documentType : "FT";
        }

        private DocumentTypeAnalyzer DocTypeAnalyzer => new DocumentTypeAnalyzer(GetDocumentType());

        private static string GetDefaultDocumentType()
        {
            return CompanyDetailsService.CompanyInformation.IsPortugal ? "FS" : "FR";
        }

        private IEnumerable<DocumentDetailDto> GetDocumentDetails()
        {
            if (_paymentMode == PaymentMode.Full)
            {
                return SaleContext.CurrentOrder.GetDocumentDetails();
            }

            return SaleItem.GetOrderDetailsFromSaleItems(_partialPaymentItems);
        }

        private IEnumerable<Api.Features.Documents.Documents.AddDocument.DocumentPaymentMethod> GetPaymentMethodsDtos()
        {
            var paymentMethods = new List<Api.Features.Documents.Documents.AddDocument.DocumentPaymentMethod>();

            if (PaymentMethod == null)
            {
                return null;
            }

            paymentMethods.Add(new Api.Features.Documents.Documents.AddDocument.DocumentPaymentMethod
            {
                PaymentMethodId = PaymentMethod.Id,
                Amount = TotalFinal
            });

            return paymentMethods;

        }

        private AddDocumentCommand CreateAddDocumentCommand()
        {
            var command = new AddDocumentCommand();

            command.Type = GetDocumentType();
            command.PaymentMethods = GetPaymentMethodsDtos();
            command.PaymentConditionId = _selectedPaymentCondition?.Id;
            command.CustomerId = (TxtCustomer.SelectedEntity as Customer)?.Id;
            command.OrderId = SaleContext.CurrentOrder.Id;
            if (command.CustomerId == null)
            {
                command.Customer = GetDocumentCustomer();
            }
            command.Discount = decimal.Parse(TxtDiscount.Text);

            if (_paymentMode == PaymentMode.Splited)
            {
                if (InitialSplittersNumber == SplittersNumber)
                {
                    SplitTickets(SplittersNumber);
                }
                if (SplittersNumber == 1)
                {
                    InitialSplittersNumber = 0;
                }
                var details = SaleContext.CurrentOrder.GetDocumentDetails().ToList();
                command.Details = details;
                return command;
            }
            command.Details = GetDocumentDetails().ToList();

            return command;
        }

        private void UncheckInvoiceMode()
        {
            _selectedPaymentCondition = null;
            BtnInvoice.Sensitive = true;
        }

        private void SelectCustomer(Customer customer)
        {
            TxtCustomer.Text = customer.Name;
            TxtCustomer.SelectedEntity = customer;
            ShowCustomerData(customer);
            FreezeEditableFields(customer.IsFinalConsumer);
        }

        private void FreezeEditableFields(bool freeze = true)
        {
            TxtFiscalNumber.BtnKeyboard.Sensitive = TxtFiscalNumber.Entry.Sensitive = !freeze;
            TxtCardNumber.BtnKeyboard.Sensitive = TxtCardNumber.Entry.Sensitive = !freeze;
            TxtCustomer.BtnKeyboard.Sensitive = TxtCustomer.Entry.Sensitive = !freeze;
            TxtAddress.BtnKeyboard.Sensitive = TxtAddress.Entry.Sensitive = !freeze;
            TxtAddress.BtnKeyboard.Sensitive = TxtLocality.Entry.Sensitive = !freeze;
            TxtCountry.BtnKeyboard.Sensitive = TxtCountry.Entry.Sensitive = !freeze;
            TxtCity.BtnKeyboard.Sensitive = TxtCity.Entry.Sensitive = !freeze;
            TxtNotes.BtnKeyboard.Sensitive = TxtNotes.Entry.Sensitive = !freeze;
            TxtZipCode.BtnKeyboard.Sensitive = TxtZipCode.Entry.Sensitive = !freeze;
        }
    }
}
