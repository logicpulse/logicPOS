using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public class PaymentsModal : Modal
    {
        private IEnumerable<PaymentMethod> _paymentMethods;
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        private PaymentMethod _selectedPaymentMethod;

        #region Components
        private IconButtonWithText BtnOk { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClearCustomer { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                           GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"),
                                                                                                           PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnNewCustomer { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("dialog_button_label_new_client"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_clients.png");
        private IconButtonWithText BtnFullPayment { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonFullPayment_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("global_button_label_payment_dialog_full_payment"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_full.png");
        private IconButtonWithText BtnPartialPayment { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea",
                                                                                                            GeneralUtils.GetResourceByName("global_button_label_payment_dialog_partial_payment"),
                                                                                                            PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_partial.png");
        private IconButtonWithText BtnCurrentAccount { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea",
                                                                                                                   GeneralUtils.GetResourceByName("global_documentfinance_type_title_ft"),
                                                                                                                  PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png");
        private IconButtonWithText BtnCurrentAccountMethod { get; set; }
        private IconButtonWithText BtnMoney { get; set; }
        private IconButtonWithText BtnCheck { get; set; }
        private IconButtonWithText BtnMB { get; set; }
        private IconButtonWithText BtnCreditCard { get; set; }
        private IconButtonWithText BtnDebitCard { get; set; }
        private IconButtonWithText BtnVisa { get; set; }
        private IconButtonWithText BtnCustomerCard { get; set; }
        private List<IconButtonWithText> PaymentMethodButtons { get; set; }

        private PageTextBox TxtCustomer { get; set; }
        private PageTextBox TxtFiscalNumber { get; set; }
        private PageTextBox TxtCountry { get; set; }
        private PageTextBox TxtDiscount { get; set; }
        private PageTextBox TxtCardNumber { get; set; }
        private PageTextBox TxtAddress { get; set; }
        private PageTextBox TxtLocality { get; set; }
        private PageTextBox TxtZipCode { get; set; }
        private PageTextBox TxtCity { get; set; }
        private PageTextBox TxtNotes { get; set; }

        private Label LabelTotal { get; } = new Label(GeneralUtils.GetResourceByName("global_total_price_to_pay") + ":");
        private Label LabelDelivery { get; } = new Label(GeneralUtils.GetResourceByName("global_total_deliver") + ":");
        private Label LabelChange { get; } = new Label(GeneralUtils.GetResourceByName("global_total_change") + ":");
        private Label LabelTotalValue { get; } = new Label("0");
        private Label LabelDeliveryValue { get; } = new Label("0");
        private Label LabelChangeValue { get; } = new Label("0");
        #endregion

        private decimal OrderTotalFinal { get; } = SaleContext.GetCurrentOrder().TotalFinal;
        private decimal TotalFinal { get; set; } = SaleContext.GetCurrentOrder().TotalFinal;
        private decimal TotalDelivery { get; set; }
        private decimal TotalChange { get; set; }

        public Guid CustomerId { get; private set; }


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

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            InitializeLabels();

            return new ActionAreaButtons
                {
                    new ActionAreaButton(BtnCurrentAccount, ResponseType.None),
                    new ActionAreaButton(BtnClearCustomer, ResponseType.None),
                    new ActionAreaButton(BtnNewCustomer, ResponseType.None),
                    new ActionAreaButton(BtnFullPayment, ResponseType.None),
                    new ActionAreaButton(BtnPartialPayment, ResponseType.None),
                    new ActionAreaButton(BtnOk, ResponseType.Ok),
                    new ActionAreaButton(BtnCancel, ResponseType.Cancel)
                };
        }

        protected override Widget CreateBody()
        {
            InitializeTextFields();
            VBox verticalLayout = new VBox(false, 0);

            HBox topPanel = new HBox(false, 0);
            topPanel.PackStart(CreatePaymentMethodsTable(), true, true, 0);
            topPanel.PackStart(CreateTotalsTable(), true, true, 0);

            verticalLayout.PackStart(topPanel, true, true, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtFiscalNumber, TxtCardNumber), true, true, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtCustomer, TxtDiscount), true, true, 0);
            verticalLayout.PackStart(TxtAddress.Component, true, true, 0);
            verticalLayout.PackStart(TxtLocality.Component, true, true, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtZipCode, TxtCity, TxtCountry), true, true, 0);
            verticalLayout.PackStart(TxtNotes.Component, true, true, 0);

            return verticalLayout;
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
                BtnCurrentAccountMethod };

            AddEventHandlers();
        }

        private void EnableAllPaymentMethodButtons()
        {
            foreach (var button in PaymentMethodButtons)
            {
                button.Sensitive = true;
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
            BtnCurrentAccountMethod.Clicked += BtnCurrentAccountMethod_Clicked;
        
            PaymentMethodButtons.ForEach( button => {button.Clicked += BtnPaymentMethod_Clicked; });

            BtnCurrentAccount.Clicked += BtnCurrentAccount_Clicked;
            BtnNewCustomer.Clicked += BtnNewCustomer_Clicked;
        }

        private void BtnNewCustomer_Clicked(object sender, EventArgs e)
        {
            Clear();
        }

        private void BtnPaymentMethod_Clicked(object sender, EventArgs e)
        {
            EnableAllPaymentMethodButtons();
            (sender as IconButtonWithText).Sensitive = false;
            UpdateTotals();
        }

        private void BtnCurrentAccount_Clicked(object sender, EventArgs e)
        {
            logicpos.Utils.ShowMessageTouch(this,
                                            DialogFlags.Modal,
                                            MessageType.Error,
                                            ButtonsType.Ok,
                                            CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_error"),
                                            CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_cant_create_cc_document_with_default_entity"));
        }

        private void BtnCurrentAccountMethod_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CURRENT_ACCOUNT");
        }

        private void BtnCustomerCard_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CUSTOMER_CARD");
        }

        private void BtnVisa_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("VISA");
        }

        private void BtnDebitCard_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("DEBIT_CARD");
        }

        private void BtnCreditCard_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CREDIT_CARD");
        }

        private void BtnMB_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("CASH_MACHINE");
        }

        private void BtnCheck_Clicked(object sender, EventArgs e)
        {
            SelectPaymentMethodByToken("BANK_CHECK");
        }

        private void BtnMoney_Clicked(object sender, EventArgs e)
        {
            InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(this, TotalFinal);
            
            if (result.Response != ResponseType.Ok)
            {
                return;
            }

            SelectPaymentMethodByToken("MONEY");

            TotalDelivery = result.Value;
        }

        private void SelectPaymentMethodByToken(string token)
        {
            _selectedPaymentMethod = _paymentMethods.FirstOrDefault(x => x.Token == token);
        }

        private void BtnClearCustomer_Clicked(object sender, EventArgs e)
        {
            Clear();
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

            BtnCurrentAccountMethod = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_current_account"),
                                                          PathsSettings.ImagesFolderLocation + @"Icons/icon_pos_payment_type_current_account.png");
        }

        private IconButtonWithText CreatePaymentMethodButton(string text, string iconPath)
        {
            var font = AppSettings.Instance.fontBaseDialogButton;
            var fontColor = AppSettings.Instance.colorBaseDialogDefaultButtonFont;
            var buttonIconSize = AppSettings.Instance.sizeBaseDialogDefaultButtonIcon;
            var buttonSize = AppSettings.Instance.sizeBaseDialogDefaultButton;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Text = text,
                    Font = font,
                    FontColor = fontColor,
                    Icon = iconPath,
                    IconSize = buttonIconSize,
                    ButtonSize = buttonSize
                });
        }

        private EventBox CreateTotalsTable()
        {
            uint padding = 9;
            Gtk.Table table = new Gtk.Table(3, 2, false);
            table.HeightRequest = 132;

            //Row 1
            table.Attach(LabelTotal, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelTotalValue, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 2
            table.Attach(LabelDelivery, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelDeliveryValue, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 3
            table.Attach(LabelChange, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelChangeValue, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            EventBox eventBox = new EventBox();
            eventBox.BorderWidth = 4;
            eventBox.ModifyBg(StateType.Normal, AppSettings.Instance.colorPosPaymentsDialogTotalPannelBackground.ToGdkColor());
            eventBox.Add(table);

            return eventBox;
        }

        private Gtk.Table CreatePaymentMethodsTable()
        {
            uint padding = 0;
            Gtk.Table table = new Gtk.Table(2, 3, true) { BorderWidth = 2 };

            //Row 1
            table.Attach(BtnMoney, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnMB, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnDebitCard, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnVisa, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 2
            table.Attach(BtnCheck, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnCreditCard, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnCurrentAccountMethod, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnCustomerCard, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            return table;
        }

        private void InitializeTextFields()
        {
            InitializeTxtCustomer();
            InitializeTxtFiscalNumber();
            InitializeTxtCardNumber();
            InitializeTxtDiscount();
            InitializeTxtAddress();
            InitializeTxtLocality();
            InitializeTxtZipCode();
            InitializeTxtCity();
            InitializeTxtCountry();
            InitializeTxtNotes();
        }

        private void InitializeTxtCountry()
        {
            TxtCountry = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_country"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtCountry.Entry.IsEditable = false;

            TxtCountry.SelectEntityClicked += BtnSelectCountry_Clicked;
        }

        private void BtnSelectCountry_Clicked(object sender, EventArgs e)
        {
            var page = new CountriesPage(null, PageOptions.SelectionPageOptions);
            var selectCountryModal = new EntitySelectionModal<Country>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCountryModal.Run();
            selectCountryModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCountry.Text = page.SelectedEntity.Designation;
                TxtCountry.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtCity()
        {
            TxtCity = new PageTextBox(this,
                                      GeneralUtils.GetResourceByName("global_city"),
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(this,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtLocality()
        {
            TxtLocality = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_locality"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_discount"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalNumber);

            TxtDiscount.Text = 0.00M.ToString("");

            TxtDiscount.Entry.Changed += (s, args) => UpdateTotals();

        }

        private void UpdateTotals()
        {
            if (!decimal.TryParse(TxtDiscount.Text, out decimal discount))
            {
                return;
            }

            var discountPrice = OrderTotalFinal * discount / 100;
            TotalFinal = OrderTotalFinal - discountPrice;

            if (_selectedPaymentMethod.Acronym != "NU")
            {
                TotalDelivery = TotalFinal;
                TotalChange = 0;
            } else
            {
                TotalChange = TotalDelivery - TotalFinal;
            }

            UpdateLabels();
        }

        private void InitializeTxtCardNumber()
        {
            TxtCardNumber = new PageTextBox(this,
                                            GeneralUtils.GetResourceByName("global_card_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtFiscalNumber()
        {
            TxtFiscalNumber = new PageTextBox(this,
                                              GeneralUtils.GetResourceByName("global_fiscal_number"),
                                              isRequired: true,
                                              isValidatable: false,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: true);

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
        }

        private void BtnSelectCustomer_Clicked(object sender, System.EventArgs e)
        {
            var page = new CustomersPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCustomer.Text = page.SelectedEntity.Name;
                TxtCustomer.SelectedEntity = page.SelectedEntity;
                CustomerId = page.SelectedEntity.Id;
                ShowCustomerData(page.SelectedEntity);
            }
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

        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Ok && response != ResponseType.Cancel)
            {
                Run();
                return;
            }

            base.OnResponse(response);
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

    }
}
