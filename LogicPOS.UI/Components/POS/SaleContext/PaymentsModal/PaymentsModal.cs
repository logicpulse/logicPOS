using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Api.Features.Documents.Documents.AddDocument;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.POS.Enums;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
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
        private readonly ISender _mediator = DependencyInjection.Mediator;
        private PaymentCondition _selectedPaymentCondition;
        private PaymentMode _paymentMode = PaymentMode.Full;
        private List<SaleItem> _partialPaymentItems = new List<SaleItem>();
        private PaymentMethod PaymentMethod => PaymentMethodsMenu.SelectedEntity;
        private decimal OrderTotalFinal { get; } = SaleContext.CurrentOrder.TotalFinal;
        private decimal TotalFinal { get; set; } = SaleContext.CurrentOrder.TotalFinal;
        private decimal TotalDelivery { get; set; }
        private decimal TotalChange { get; set; }
        private int SplittersNumber;
        public string paymentMethodDesignation{get;set;}
        public PaymentsModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_payments"),
                                                   new Size(640, 700),
                                                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_payments.png")
        {
      
            UpdateLabels();
        }
        public PaymentsModal(Window parent, Customer customer,int splittersNumber=2) : base(parent,
                                           GeneralUtils.GetResourceByName("window_title_dialog_payments"),
                                           new Size(640, 700),
                                           AppSettings.Paths.Images + @"Icons\Windows\icon_window_payments.png")
        {
            SplitModeInitilizer(customer, splittersNumber);
            ShowCustomerData(TxtCustomer.SelectedEntity as Customer);
            UpdateLabels();
        }

        private void SplitModeInitilizer(Customer customer, int splittersNumber)
        {
            SplittersNumber = splittersNumber;
            _paymentMode = PaymentMode.Splited;
            TxtCustomer.Component.Sensitive = false;
            TxtCardNumber.Entry.Sensitive = false;
            TxtAddress.Entry.Sensitive = false;
            TxtCity.Entry.Sensitive = false;
            TxtCountry.Entry.Sensitive = false;
            TxtFiscalNumber.Entry.Sensitive = false;
            TxtLocality.Entry.Sensitive = false;
            TxtZipCode.Entry.Sensitive = false;
            TxtCustomer.SelectedEntity = customer;
            TxtCustomer.Text = customer.Name;
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

        private void EnableAllPaymentMethodButtons(bool enable = true)
        {
            foreach (var button in PaymentMethodsMenu.Buttons)
            {
                button.Button.Sensitive = enable;
            }
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
                    TotalFinal = OrderTotalFinal / SplittersNumber;
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
                Country = country.Code2,
                CountryId = country.Id
            };
        }

        private string GetDocumentType()
        {
            return BtnInvoice.Sensitive ? "FR" : "FT";
        }

        private IEnumerable<DocumentDetailDto> GetDocumentDetails()
        {   
            if(_paymentMode == PaymentMode.Full)
            {
                return SaleContext.CurrentOrder.GetDocumentDetails();
            }

            return SaleItem.GetOrderDetailsFromSaleItems(_partialPaymentItems);
        }

        private IEnumerable<AddDocumentPaymentMethodDto> GetPaymentMethodsDtos()
        {
            var paymentMethods = new List<AddDocumentPaymentMethodDto>();

            if (PaymentMethod == null)
            {
                return null;
            }

            paymentMethods.Add(new AddDocumentPaymentMethodDto
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
            if (command.CustomerId == null)
            {
                command.Customer = GetDocumentCustomer();
            }
            command.Discount = decimal.Parse(TxtDiscount.Text);

            if (_paymentMode == PaymentMode.Splited)
            {
                var details = SaleContext.CurrentOrder.GetDocumentDetails().ToList();
                foreach (var item in details)
                {
                    item.Quantity = item.Quantity / SplittersNumber;
                }
                
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
    }
}
