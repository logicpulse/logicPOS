using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.Article;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PaymentDialog : BaseDialog
    {
        //Settings
        private readonly Color _colorEntryValidationValidFont = AppSettings.Instance.colorEntryValidationValidFont;
        private readonly Color _colorEntryValidationInvalidFont = AppSettings.Instance.colorEntryValidationInvalidFontLighter;
        //Usefull to block .Change events when we change Entry.Text from code, and prevent to recursivly call Change Events
        private bool _enableGetCustomerDetails = true;
        //PartialPayments Stuff
        private PosSelectRecordDialog<DataTable, DataRow, TreeViewPartialPayment> _dialogPartialPayment;
        private decimal _totalPartialPaymentItems = 0;
        //Default DocumentType (FS)
        private Guid _processDocumentType = DocumentSettings.SimplifiedInvoiceId;
        //ResponseType (Above 10)
        private readonly ResponseType _responseTypeClearCustomer = (ResponseType)11;
        private readonly ResponseType _responseTypeFullPayment = (ResponseType)12;
        private readonly ResponseType _responseTypePartialPayment = (ResponseType)13;
        private readonly ResponseType _responseTypeCurrentAccount = (ResponseType)14;
        //UI
        private readonly Label _labelTotalValue;
        private readonly Label _labelDeliveryValue;
        private readonly Label _labelChangeValue;
        //UI EntryBox
        private readonly XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerFiscalNumber;
        private readonly XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerCardNumber;
        private readonly XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerName;
        private readonly XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, GridViewCountries> _entryBoxSelectCustomerCountry;
        private readonly EntryBoxValidation _entryBoxCustomerDiscount;
        private readonly EntryBoxValidation _entryBoxCustomerAddress;
        private readonly EntryBoxValidation _entryBoxCustomerLocality;
        private readonly EntryBoxValidation _entryBoxCustomerZipCode;
        private readonly EntryBoxValidation _entryBoxCustomerCity;
        private readonly EntryBoxValidation _entryBoxCustomerNotes;
        //ActionArea
        private readonly IconButtonWithText _buttonOk;
        private readonly IconButtonWithText _buttonCancel;
        private readonly IconButtonWithText _buttonClearCustomer;
        private readonly IconButtonWithText _buttonNewCustomer;
        private readonly IconButtonWithText _buttonFullPayment;
        private readonly IconButtonWithText _buttonPartialPayment;
        private readonly IconButtonWithText _buttonCurrentAccount;
        //Default Objects
        private readonly cfg_configurationcountry _intialValueConfigurationCountry;
        //Store Partial Payment Enabled/Disabled
        private bool _partialPaymentEnabled = false;
        //Store PrequestProcessFinanceDocumentParameter
        private readonly bool _skipPersistFinanceDocument;

        public decimal TotalDelivery { get; set; } = 0.0m;

        public decimal TotalChange { get; set; } = 0.0m;

        public decimal DiscountGlobal { get; set; } = 0.0m;

        public CustomButton SelectedPaymentMethodButton { get; set; }
        public fin_configurationpaymentmethod PaymentMethod { get; set; }

        public erp_customer Customer { get; set; }
        public cfg_configurationcountry Country { get; set; }

        public ArticleBag ArticleBagFullPayment { get; set; }
        public ArticleBag ArticleBagPartialPayment { get; set; }

        public DocumentProcessingParameters ProcessFinanceDocumentParameter { get; set; }

        //Constructors
        public PaymentDialog(
            Window parentWindow,
            DialogFlags pDialogFlags,
            ArticleBag pArticleBag)
            : this(parentWindow, pDialogFlags, pArticleBag, true) { }

        public PaymentDialog(
            Window parentWindow,
            DialogFlags pDialogFlags,
            ArticleBag pArticleBag,
            bool pEnablePartialPaymentButtons)
            : this(parentWindow, pDialogFlags, pArticleBag, pEnablePartialPaymentButtons, true) { }

        /* Please see ERR201810#14 */
        // A call to this overloaded contructor have issues when user is paying "Conta Corrente", when it is causing 2 invoices being created.
        // We changed the call to this overloaded method, directing the flow to main constructor from calling class.
        public PaymentDialog(
            Window parentWindow,
            DialogFlags pDialogFlags,
            ArticleBag pArticleBag,
            bool pEnablePartialPaymentButtons,
            bool pEnableCurrentAccountButton)
            : this(parentWindow, pDialogFlags, pArticleBag, pEnablePartialPaymentButtons, pEnableCurrentAccountButton, false, null, null) { }


        public PaymentDialog(
            Window parentWindow,
            DialogFlags pDialogFlags,
            ArticleBag pArticleBag,
            bool pEnablePartialPaymentButtons,
            bool pEnableCurrentAccountButton,
            bool pSkipPersistFinanceDocument,
            DocumentProcessingParameters pProcessFinanceDocumentParameter,
            string pSelectedPaymentMethodButtonName)
            : base(parentWindow, pDialogFlags, false)
        {
            //Init Local Vars
            WindowSettings.Source = parentWindow;
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_payments");
            //TODO:THEME
            Size windowSize = new Size(633, 620);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_payments.png";

            //Parameters
            ArticleBagFullPayment = pArticleBag;
            _skipPersistFinanceDocument = pSkipPersistFinanceDocument;
            ProcessFinanceDocumentParameter = pProcessFinanceDocumentParameter;
            bool enablePartialPaymentButtons = true;
            bool enableCurrentAccountButton = pEnableCurrentAccountButton;
            //if (enablePartialPaymentButtons) enablePartialPaymentButtons = (_articleBagFullPayment.TotalQuantity > 1) ? true : false;
            //Files
            //TK016311 Botão Novo Cliente nos pagamentos do TicketPad 
            string fileIconNewCustomer = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_clients.png";
            string fileIconClearCustomer = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png";
            string fileIconFullPayment = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_full.png";
            string fileIconPartialPayment = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_partial.png";
            string fileIconCurrentAccount = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png";
            //Valor a pagar 
            //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
            string fileIconChangePaumentAmount = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_pos_toolbar_finance_document.png";

            //Colors
            Color colorPosPaymentsDialogTotalPannelBackground = AppSettings.Instance.colorPosPaymentsDialogTotalPannelBackground;
            //Objects
            _intialValueConfigurationCountry = XPOSettings.ConfigurationSystemCountry;

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Payment Buttons
            //Get Custom Select Data
            string executeSql = @"SELECT Oid, Token, ResourceString FROM fin_configurationpaymentmethod ORDER BY Ord;";
            SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(executeSql);
            //Get Required XpObjects from Selected Data
            fin_configurationpaymentmethod xpoMoney = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "MONEY");
            fin_configurationpaymentmethod xpoCheck = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "BANK_CHECK");
            fin_configurationpaymentmethod xpoMB = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "CASH_MACHINE");
            fin_configurationpaymentmethod xpoCreditCard = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "CREDIT_CARD");
            /* IN009142 - "Visa" option replaced by "Debit Card" */
            fin_configurationpaymentmethod xpoDebitCard = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "DEBIT_CARD");
            fin_configurationpaymentmethod xpoVisa = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "VISA");
            fin_configurationpaymentmethod xpoCurrentAccount = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "CURRENT_ACCOUNT");
            fin_configurationpaymentmethod xpoCustomerCard = (fin_configurationpaymentmethod)xPSelectData.GetXPGuidObjectFromField(typeof(fin_configurationpaymentmethod), "Token", "CUSTOMER_CARD");

            //Instantiate Buttons  //IN009257 Redimensionar botões para a resolução 1024 x 768. Alterei variável de tamanho de icons. era a mesma que documentos
            IconButtonWithText buttonMoney = new IconButtonWithText(new ButtonSettings { Name = "touchButtonMoney_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoMoney.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoMoney.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.PaymentButton }) { CurrentButtonId = xpoMoney.Oid, Sensitive = !xpoMoney.Disabled };
            IconButtonWithText buttonCheck = new IconButtonWithText(new ButtonSettings { Name = "touchButtonCheck_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoCheck.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoCheck.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.PaymentButton }) { CurrentButtonId = xpoCheck.Oid, Sensitive = !xpoCheck.Disabled };
            IconButtonWithText buttonMB = new IconButtonWithText(new ButtonSettings { Name = "touchButtonMB_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoMB.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoMB.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.PaymentButton }) { CurrentButtonId = xpoMB.Oid, Sensitive = !xpoMB.Disabled };
            IconButtonWithText buttonCreditCard = new IconButtonWithText(new ButtonSettings { Name = "touchButtonCreditCard_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoCreditCard.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoCreditCard.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.PaymentButton }) { CurrentButtonId = xpoCreditCard.Oid, Sensitive = !xpoCreditCard.Disabled };
            /* IN009142 */
            IconButtonWithText buttonDebitCard = new IconButtonWithText(new ButtonSettings { Name = "touchButtonDebitCard_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoDebitCard.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoDebitCard.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.PaymentButton }) { CurrentButtonId = xpoDebitCard.Oid, Sensitive = !xpoDebitCard.Disabled };
            IconButtonWithText buttonVisa = new IconButtonWithText(new ButtonSettings { Name = "touchButtonVisa_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoVisa.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoVisa.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.PaymentButton }) { CurrentButtonId = xpoVisa.Oid, Sensitive = !xpoVisa.Disabled };
            IconButtonWithText buttonCurrentAccount = new IconButtonWithText(new ButtonSettings { Name = "touchButtonCurrentAccount_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoCurrentAccount.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoCurrentAccount.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.DefaultButton }) { CurrentButtonId = xpoCurrentAccount.Oid, Sensitive = !xpoCurrentAccount.Disabled };
            IconButtonWithText buttonCustomerCard = new IconButtonWithText(new ButtonSettings { Name = "touchButtonCustomerCard_Green", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, xpoCustomerCard.ResourceString), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = $"{PathsSettings.ImagesFolderLocation}{xpoCustomerCard.ButtonIcon}", IconSize = SizeSettings.PaymentButtonIcon, ButtonSize = SizeSettings.DefaultButton }) { CurrentButtonId = xpoCustomerCard.Oid, Sensitive = !xpoCustomerCard.Disabled };
            //Secondary Buttons
            //Events
            buttonMoney.Clicked += buttonMoney_Clicked;
            buttonCheck.Clicked += buttonCheck_Clicked;
            buttonMB.Clicked += buttonMB_Clicked;
            buttonCreditCard.Clicked += buttonCredit_Clicked;
            /* IN009142 */
            buttonDebitCard.Clicked += buttonDebitCard_Clicked;
            buttonVisa.Clicked += buttonVisa_Clicked;
            buttonCurrentAccount.Clicked += buttonCurrentAccount_Clicked;
            buttonCustomerCard.Clicked += buttonCustomerCard_Clicked;

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

            //Table
            uint tablePaymentsPadding = 0;
            Table tablePayments = new Table(2, 3, true) { BorderWidth = 2 };
            //Row 1
            tablePayments.Attach(buttonMoney, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
            tablePayments.Attach(buttonMB, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
            /* IN009142 - adding "Debit Card" option to Payment window */
            tablePayments.Attach(buttonDebitCard, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
            /* IN009142 - removed "Visa" payment method */
            //tablePayments.Attach(buttonVisa, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
            //Row 2
            tablePayments.Attach(buttonCheck, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
            tablePayments.Attach(buttonCreditCard, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
            if (enableCurrentAccountButton) tablePayments.Attach(
                (LogicPOSSettings.PosPaymentsDialogUseCurrentAccount) ? buttonCurrentAccount : buttonCustomerCard
                , 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding
            );

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

            //Labels
            Label labelTotal = new Label(GeneralUtils.GetResourceByName("global_total_price_to_pay") + ":");
            Label labelDelivery = new Label(GeneralUtils.GetResourceByName("global_total_deliver") + ":");
            Label labelChange = new Label(GeneralUtils.GetResourceByName("global_total_change") + ":");
            _labelTotalValue = new Label(DataConversionUtils.DecimalToStringCurrency(ArticleBagFullPayment.TotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym))
            {
                //Total Width
                WidthRequest = 135
            };
            _labelDeliveryValue = new Label(DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym));
            _labelChangeValue = new Label(DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym));

            //Colors
            labelTotal.ModifyFg(StateType.Normal, Color.FromArgb(101, 137, 171).ToGdkColor());
            labelDelivery.ModifyFg(StateType.Normal, Color.FromArgb(101, 137, 171).ToGdkColor());
            labelChange.ModifyFg(StateType.Normal, Color.FromArgb(101, 137, 171).ToGdkColor());
            _labelTotalValue.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
            _labelDeliveryValue.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
            _labelChangeValue.ModifyFg(StateType.Normal, Color.White.ToGdkColor());

            //Alignments
            labelTotal.SetAlignment(0, 0.5F);
            labelDelivery.SetAlignment(0, 0.5F);
            labelChange.SetAlignment(0, 0.5F);
            _labelTotalValue.SetAlignment(1, 0.5F);
            _labelDeliveryValue.SetAlignment(1, 0.5F);
            _labelChangeValue.SetAlignment(1, 0.5F);

            //labels Font
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString("Bold 10");
            labelTotal.ModifyFont(fontDescription);
            labelDelivery.ModifyFont(fontDescription);
            labelChange.ModifyFont(fontDescription);
            Pango.FontDescription fontDescriptionValue = Pango.FontDescription.FromString("Bold 12");
            _labelTotalValue.ModifyFont(fontDescriptionValue);
            _labelDeliveryValue.ModifyFont(fontDescriptionValue);
            _labelChangeValue.ModifyFont(fontDescriptionValue);

            //Table TotalPannel
            uint totalPannelPadding = 9;
            Table tableTotalPannel = new Table(3, 2, false);
            tableTotalPannel.HeightRequest = 132;
            //Row 1
            tableTotalPannel.Attach(labelTotal, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, totalPannelPadding, totalPannelPadding);
            tableTotalPannel.Attach(_labelTotalValue, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, totalPannelPadding, totalPannelPadding);
            //Row 2
            tableTotalPannel.Attach(labelDelivery, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, totalPannelPadding, totalPannelPadding);
            tableTotalPannel.Attach(_labelDeliveryValue, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, totalPannelPadding, totalPannelPadding);
            //Row 3
            tableTotalPannel.Attach(labelChange, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, totalPannelPadding, totalPannelPadding);
            tableTotalPannel.Attach(_labelChangeValue, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, totalPannelPadding, totalPannelPadding);

            //TotalPannel
            EventBox eventboxTotalPannel = new EventBox();
            eventboxTotalPannel.BorderWidth = 4;
            eventboxTotalPannel.ModifyBg(StateType.Normal, colorPosPaymentsDialogTotalPannelBackground.ToGdkColor());
            eventboxTotalPannel.Add(tableTotalPannel);

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

            //Customer Name
            CriteriaOperator criteriaOperatorCustomerName = null;
            /* IN009202 */
            _entryBoxSelectCustomerName = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(WindowSettings.Source, GeneralUtils.GetResourceByName("global_customer"), "Name", "Name", null, criteriaOperatorCustomerName, KeyboardMode.Alfa, RegexUtils.RegexAlfaNumericPlus, false);
            _entryBoxSelectCustomerName.ClosePopup += delegate
            {
                //IN009284 POS - Pagamento conta-corrente - Cliente por defeito 
                if (ProcessFinanceDocumentParameter != null)
                {
                    GetCustomerDetails("Oid", ProcessFinanceDocumentParameter.Customer.ToString());
                    Validate();
                }
                //IN009284 ENDS
                else
                {
                    GetCustomerDetails("Oid", _entryBoxSelectCustomerName.Value.Oid.ToString());
                    Validate();
                }
            };
            _entryBoxSelectCustomerName.EntryValidation.Changed += _entryBoxSelectCustomerName_Changed;

            //Customer Discount
            _entryBoxCustomerDiscount = new EntryBoxValidation(WindowSettings.Source, GeneralUtils.GetResourceByName("global_discount"), KeyboardMode.Alfa, RegexUtils.RegexPercentage, true);
            _entryBoxCustomerDiscount.EntryValidation.Text = DataConversionUtils.DecimalToString(0.0m);
            _entryBoxCustomerDiscount.EntryValidation.Sensitive = false;
            _entryBoxCustomerDiscount.EntryValidation.Changed += _entryBoxCustomerDiscount_Changed;
            _entryBoxCustomerDiscount.EntryValidation.FocusOutEvent += delegate
            {
                _entryBoxCustomerDiscount.EntryValidation.Text = DataConversionUtils.StringToDecimalAndToStringAgain(_entryBoxCustomerDiscount.EntryValidation.Text);
            };

            //Address
            _entryBoxCustomerAddress = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_address"), KeyboardMode.Alfa, RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            _entryBoxCustomerAddress.EntryValidation.Changed += delegate { Validate(); };

            //Locality
            _entryBoxCustomerLocality = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_locality"), KeyboardMode.Alfa, RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            _entryBoxCustomerLocality.EntryValidation.Changed += delegate { Validate(); };

            //ZipCode
            _entryBoxCustomerZipCode = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_zipcode"), KeyboardMode.Alfa, XPOSettings.ConfigurationSystemCountry.RegExZipCode, false);
            _entryBoxCustomerZipCode.WidthRequest = 150;
            _entryBoxCustomerZipCode.EntryValidation.Changed += delegate { Validate(); };

            //City
            _entryBoxCustomerCity = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_city"), KeyboardMode.Alfa, RegexUtils.RegexAlfaNumericPlus, false);/* IN009253 */
            _entryBoxCustomerCity.WidthRequest = 200;
            _entryBoxCustomerCity.EntryValidation.Changed += delegate { Validate(); };

            //Country
            CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (RegExFiscalNumber IS NOT NULL AND RegExZipCode IS NOT NULL)");
            _entryBoxSelectCustomerCountry = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, GridViewCountries>(parentWindow, GeneralUtils.GetResourceByName("global_country"), "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, RegexUtils.RegexGuid, true);
            _entryBoxSelectCustomerCountry.WidthRequest = 235;
            //Extra Protection to prevent Customer without Country
            if (_entryBoxSelectCustomerCountry.Value != null) _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
            _entryBoxSelectCustomerCountry.EntryValidation.IsEditable = false;
            _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = false;
            _entryBoxSelectCustomerCountry.ClosePopup += delegate
            {
                Country = _entryBoxSelectCustomerCountry.Value;
                //Require to Update RegEx and Criteria to filter Country Clients Only
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExFiscalNumber;
                _entryBoxCustomerZipCode.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExZipCode;
                //Clear Customer Fields, Except Country
                ClearCustomer(false);
                //Apply Criteria Operators
                ApplyCriteriaToCustomerInputs();
                //Call Main Validate
                Validate();
            };

            //FiscalNumber
            CriteriaOperator criteriaOperatorFiscalNumber = null;
            _entryBoxSelectCustomerFiscalNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(WindowSettings.Source, GeneralUtils.GetResourceByName("global_fiscal_number"), "FiscalNumber", "FiscalNumber", null, criteriaOperatorFiscalNumber, KeyboardMode.AlfaNumeric, _intialValueConfigurationCountry.RegExFiscalNumber, false);
            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Changed += _entryBoxSelectCustomerFiscalNumber_Changed;

            //CardNumber
            CriteriaOperator criteriaOperatorCardNumber = null;//Now Criteria is assigned in ApplyCriteriaToCustomerInputs();
            _entryBoxSelectCustomerCardNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(WindowSettings.Source, GeneralUtils.GetResourceByName("global_card_number"), "CardNumber", "CardNumber", null, criteriaOperatorCardNumber, KeyboardMode.AlfaNumeric, RegexUtils.RegexAlfaNumericExtended, false);
            _entryBoxSelectCustomerCardNumber.ClosePopup += delegate
            {
                if (_entryBoxSelectCustomerCardNumber.EntryValidation.Validated) GetCustomerDetails("CardNumber", _entryBoxSelectCustomerCardNumber.EntryValidation.Text);
                Validate();
            };

            //Notes
            _entryBoxCustomerNotes = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_notes"), KeyboardMode.Alfa, RegexUtils.RegexAlfaNumericExtended, false);
            _entryBoxCustomerNotes.EntryValidation.Changed += delegate { Validate(); };

            //Fill Dialog Inputs with Defaults FinalConsumerEntity Values
            if (ProcessFinanceDocumentParameter == null)
            {
                //If ProcessFinanceDocumentParameter is not null fill Dialog with value from ProcessFinanceDocumentParameter, implemented for SplitPayments
                GetCustomerDetails("Oid", InvoiceSettings.FinalConsumerId.ToString());
            }
            //Fill Dialog Inputs with Stored Values, ex when we Work with SplitPayments
            else
            {
                //Apply Default Customer Entity
                GetCustomerDetails("Oid", ProcessFinanceDocumentParameter.Customer.ToString());

                //Assign Totasl and Discounts Values
                TotalDelivery = ProcessFinanceDocumentParameter.TotalDelivery;
                TotalChange = ProcessFinanceDocumentParameter.TotalChange;
                DiscountGlobal = ProcessFinanceDocumentParameter.ArticleBag.DiscountGlobal;
                // Update Visual Components
                _labelDeliveryValue.Text = DataConversionUtils.DecimalToStringCurrency(TotalDelivery, XPOSettings.ConfigurationSystemCurrency.Acronym);
                _labelChangeValue.Text = DataConversionUtils.DecimalToStringCurrency(TotalChange, XPOSettings.ConfigurationSystemCurrency.Acronym);
                // Selects
                Customer = XPOUtility.GetEntityById<erp_customer>(ProcessFinanceDocumentParameter.Customer);
                Country = Customer.Country;
                // PaymentMethod
                PaymentMethod = XPOUtility.GetEntityById<fin_configurationpaymentmethod>(ProcessFinanceDocumentParameter.PaymentMethod);
                // Restore Selected Payment Method, require to associate button reference to selectedPaymentMethodButton
                if (!string.IsNullOrEmpty(pSelectedPaymentMethodButtonName))
                {
                    switch (pSelectedPaymentMethodButtonName)
                    {
                        case "touchButtonMoney_Green":
                            SelectedPaymentMethodButton = buttonMoney;
                            break;
                        case "touchButtonCheck_Green":
                            SelectedPaymentMethodButton = buttonCheck;
                            break;
                        case "touchButtonMB_Green":
                            SelectedPaymentMethodButton = buttonMB;
                            break;
                        case "touchButtonCreditCard_Green":
                            SelectedPaymentMethodButton = buttonCreditCard;
                            break;
                        /* IN009142 */
                        case "touchButtonDebitCard_Green":
                            SelectedPaymentMethodButton = buttonDebitCard;
                            break;
                        case "touchButtonVisa_Green":
                            SelectedPaymentMethodButton = buttonVisa;
                            break;
                        case "touchButtonCurrentAccount_Green":
                            SelectedPaymentMethodButton = buttonCurrentAccount;
                            break;
                        case "touchButtonCustomerCard_Green":
                            SelectedPaymentMethodButton = buttonCustomerCard;
                            break;
                    }

                    //Assign Payment Method after have Reference
                    AssignPaymentMethod(SelectedPaymentMethodButton);
                }

                //UpdateChangeValue, if we add/remove Splits we must recalculate ChangeValue
                UpdateChangeValue();
            }

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

            //Pack Content

            HBox hboxPaymenstAndTotals = new HBox(false, 0);
            hboxPaymenstAndTotals.PackStart(tablePayments, true, true, 0);
            hboxPaymenstAndTotals.PackStart(eventboxTotalPannel, true, true, 0);

            HBox hboxCustomerNameAndCustomerDiscount = new HBox(true, 0);
            hboxCustomerNameAndCustomerDiscount.PackStart(_entryBoxSelectCustomerName, true, true, 0);
            hboxCustomerNameAndCustomerDiscount.PackStart(_entryBoxCustomerDiscount, true, true, 0);

            HBox hboxFiscalNumberAndCardNumber = new HBox(true, 0);
            hboxFiscalNumberAndCardNumber.PackStart(_entryBoxSelectCustomerFiscalNumber, true, true, 0);
            hboxFiscalNumberAndCardNumber.PackStart(_entryBoxSelectCustomerCardNumber, true, true, 0);

            HBox hboxZipCodeCityAndCountry = new HBox(false, 0);
            hboxZipCodeCityAndCountry.PackStart(_entryBoxCustomerZipCode, false, false, 0);
            hboxZipCodeCityAndCountry.PackStart(_entryBoxCustomerCity, false, false, 0);
            hboxZipCodeCityAndCountry.PackStart(_entryBoxSelectCustomerCountry, true, true, 0);

            VBox vboxContent = new VBox(false, 0);
            vboxContent.PackStart(hboxPaymenstAndTotals, true, true, 0);
            vboxContent.PackStart(hboxFiscalNumberAndCardNumber, true, true, 0);
            vboxContent.PackStart(hboxCustomerNameAndCustomerDiscount, true, true, 0);
            vboxContent.PackStart(_entryBoxCustomerAddress, true, true, 0);
            vboxContent.PackStart(_entryBoxCustomerLocality, true, true, 0);
            vboxContent.PackStart(hboxZipCodeCityAndCountry, true, true, 0);
            vboxContent.PackStart(_entryBoxCustomerNotes, true, true, 0);

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            _buttonClearCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"), fileIconClearCustomer);
            _buttonNewCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", GeneralUtils.GetResourceByName("dialog_button_label_new_client"), fileIconNewCustomer);
            _buttonFullPayment = ActionAreaButton.FactoryGetDialogButtonType("touchButtonFullPayment_DialogActionArea", GeneralUtils.GetResourceByName("global_button_label_payment_dialog_full_payment"), fileIconFullPayment);
            _buttonPartialPayment = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea", GeneralUtils.GetResourceByName("global_button_label_payment_dialog_partial_payment"), fileIconPartialPayment);
            _buttonCurrentAccount = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea", GeneralUtils.GetResourceByName("global_documentfinance_type_title_ft"), fileIconCurrentAccount);
            // Enable if has selectedPaymentMethod defined, ex when working with SplitPayments
            _buttonOk.Sensitive = (PaymentMethod != null);
            _buttonFullPayment.Sensitive = false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
                {
                    new ActionAreaButton(_buttonCurrentAccount, _responseTypeCurrentAccount),
                    new ActionAreaButton(_buttonClearCustomer, _responseTypeClearCustomer),
                    new ActionAreaButton(_buttonNewCustomer, _responseTypeClearCustomer)
                };
            if (enablePartialPaymentButtons)
            {
                actionAreaButtons.Add(new ActionAreaButton(_buttonFullPayment, _responseTypeFullPayment));
                actionAreaButtons.Add(new ActionAreaButton(_buttonPartialPayment, _responseTypePartialPayment));
            }

            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, vboxContent, actionAreaButtons);

        }
    }
}