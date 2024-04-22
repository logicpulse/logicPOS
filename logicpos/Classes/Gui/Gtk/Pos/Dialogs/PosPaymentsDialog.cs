using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Extensions;
using logicpos.financial.library.Classes.Finance;
using logicpos.resources.Resources.Localization;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using System;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosPaymentsDialog : PosBaseDialog
    {
        //Settings
        private readonly Color _colorEntryValidationValidFont = DataLayerFramework.Settings["colorEntryValidationValidFont"].StringToColor();
        private readonly Color _colorEntryValidationInvalidFont = DataLayerFramework.Settings["colorEntryValidationInvalidFontLighter"].StringToColor();
        //Usefull to block .Change events when we change Entry.Text from code, and prevent to recursivly call Change Events
        private bool _enableGetCustomerDetails = true;
        //PartialPayments Stuff
        private PosSelectRecordDialog<DataTable, DataRow, TreeViewPartialPayment> _dialogPartialPayment;
        private decimal _totalPartialPaymentItems = 0;
        //Default DocumentType (FS)
        private Guid _processDocumentType = SharedSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice;
        //ResponseType (Above 10)
        private readonly ResponseType _responseTypeClearCustomer = (ResponseType)11;
        private readonly ResponseType _responseTypeFullPayment = (ResponseType)12;
        private readonly ResponseType _responseTypePartialPayment = (ResponseType)13;
        private readonly ResponseType _responseTypeCurrentAccount= (ResponseType)14;
        //UI
        private readonly Label _labelTotalValue;
        private readonly Label _labelDeliveryValue;
        private readonly Label _labelChangeValue;
        //UI EntryBox
        private readonly XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerFiscalNumber;
        private readonly XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerCardNumber;
        private readonly XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectCustomerName;
        private readonly XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectCustomerCountry;
        private readonly EntryBoxValidation _entryBoxCustomerDiscount;
        private readonly EntryBoxValidation _entryBoxCustomerAddress;
        private readonly EntryBoxValidation _entryBoxCustomerLocality;
        private readonly EntryBoxValidation _entryBoxCustomerZipCode;
        private readonly EntryBoxValidation _entryBoxCustomerCity;
        private readonly EntryBoxValidation _entryBoxCustomerNotes;
        //ActionArea
        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;
        private readonly TouchButtonIconWithText _buttonClearCustomer;
        private readonly TouchButtonIconWithText _buttonNewCustomer;
        private readonly TouchButtonIconWithText _buttonFullPayment;
        private readonly TouchButtonIconWithText _buttonPartialPayment;
        private readonly TouchButtonIconWithText _buttonCurrentAccount;
        //Default Objects
        private readonly cfg_configurationcountry _intialValueConfigurationCountry;
        //Store Partial Payment Enabled/Disabled
        private bool _partialPaymentEnabled = false;
        //Store PrequestProcessFinanceDocumentParameter
        private readonly bool _skipPersistFinanceDocument;
        //Public Properties
        private decimal _totalDelivery = 0.0m;
        public decimal TotalDelivery
        {
            get { return _totalDelivery; }
            set { _totalDelivery = value; }
        }
        private decimal _totalChange = 0.0m;
        public decimal TotalChange
        {
            get { return _totalChange; }
            set { _totalChange = value; }
        }
        private decimal _discountGlobal = 0.0m;
        public decimal DiscountGlobal
        {
            get { return _discountGlobal; }
            set { _discountGlobal = value; }
        }
        private TouchButtonBase _selectedPaymentMethodButton;
        public TouchButtonBase SelectedPaymentMethodButton 
        { 
            get { return _selectedPaymentMethodButton; }
            set { _selectedPaymentMethodButton = value; }
        }
        private fin_configurationpaymentmethod _selectedPaymentMethod;
        public fin_configurationpaymentmethod PaymentMethod
        {
            get { return _selectedPaymentMethod; }
            set { _selectedPaymentMethod = value; }
        }
        private erp_customer _selectedCustomer;
        public erp_customer Customer
        {
            get { return _selectedCustomer; }
            set { _selectedCustomer = value; }
        }
        private cfg_configurationcountry _selectedCountry;
        public cfg_configurationcountry Country
        {
            get { return _selectedCountry; }
            set { _selectedCountry = value; }
        }
        private ArticleBag _articleBagFullPayment;
        public ArticleBag ArticleBagFullPayment
        {
            get { return _articleBagFullPayment; }
            set { _articleBagFullPayment = value; }
        }
        private ArticleBag _articleBagPartialPayment;
        public ArticleBag ArticleBagPartialPayment
        {
            get { return _articleBagPartialPayment; }
            set { _articleBagPartialPayment = value; }
        }
        // To be used in SplitPayments on Call this Window
        private ProcessFinanceDocumentParameter _processFinanceDocumentParameter;
        public ProcessFinanceDocumentParameter ProcessFinanceDocumentParameter
        {
            get { return _processFinanceDocumentParameter; }
            set { _processFinanceDocumentParameter = value; }
        }

        //Constructors
        public PosPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag pArticleBag)
            : this(pSourceWindow, pDialogFlags, pArticleBag, true) { }

        public PosPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag pArticleBag, bool pEnablePartialPaymentButtons)
            : this(pSourceWindow, pDialogFlags, pArticleBag, pEnablePartialPaymentButtons, true) { }

        /* Please see ERR201810#14 */
        // A call to this overloaded contructor have issues when user is paying "Conta Corrente", when it is causing 2 invoices being created.
        // We changed the call to this overloaded method, directing the flow to main constructor from calling class.
        public PosPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag pArticleBag, bool pEnablePartialPaymentButtons, bool pEnableCurrentAccountButton)
            : this(pSourceWindow, pDialogFlags, pArticleBag, pEnablePartialPaymentButtons, pEnableCurrentAccountButton, false, null, null) { }



        public PosPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag pArticleBag, bool pEnablePartialPaymentButtons, bool pEnableCurrentAccountButton, bool pSkipPersistFinanceDocument, ProcessFinanceDocumentParameter pProcessFinanceDocumentParameter, string pSelectedPaymentMethodButtonName)
            : base(pSourceWindow, pDialogFlags, false)
        {
            try
            {
                //Init Local Vars
                _sourceWindow = pSourceWindow;
                string windowTitle = resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_payments");
                //TODO:THEME
                Size windowSize = new Size(633, 620);
                string fileDefaultWindowIcon = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_payments.png");

                //Parameters
                _articleBagFullPayment = pArticleBag;
                _skipPersistFinanceDocument = pSkipPersistFinanceDocument;
                _processFinanceDocumentParameter = pProcessFinanceDocumentParameter;
                bool enablePartialPaymentButtons = true;
                bool enableCurrentAccountButton = pEnableCurrentAccountButton;
                //if (enablePartialPaymentButtons) enablePartialPaymentButtons = (_articleBagFullPayment.TotalQuantity > 1) ? true : false;
                //Files
				//TK016311 Botão Novo Cliente nos pagamentos do TicketPad 
                string fileIconNewCustomer = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_clients.png");
                string fileIconClearCustomer = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_nav_delete.png");
                string fileIconFullPayment = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_payment_full.png");
                string fileIconPartialPayment = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_payment_partial.png");
                string fileIconCurrentAccount = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\icon_pos_toolbar_finance_document.png");
                //Valor a pagar 
                //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
               string fileIconChangePaumentAmount = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\BackOffice\icon_pos_toolbar_finance_document.png");

                //Colors
                Color colorPosPaymentsDialogTotalPannelBackground = DataLayerFramework.Settings["colorPosPaymentsDialogTotalPannelBackground"].StringToColor();
                //Objects
                _intialValueConfigurationCountry = DataLayerSettings.ConfigurationSystemCountry;

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Payment Buttons
                //Get Custom Select Data
                string executeSql = @"SELECT Oid, Token, ResourceString FROM fin_configurationpaymentmethod ORDER BY Ord;";
                XPSelectData xPSelectData = SharedUtils.GetSelectedDataFromQuery(executeSql);
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
                TouchButtonIconWithText buttonMoney = new TouchButtonIconWithText("touchButtonMoney_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoMoney.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoMoney.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBasePaymentButton.Width, _sizeBasePaymentButton.Height) { CurrentButtonOid = xpoMoney.Oid, Sensitive = !xpoMoney.Disabled };
                TouchButtonIconWithText buttonCheck = new TouchButtonIconWithText("touchButtonCheck_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoCheck.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoCheck.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBasePaymentButton.Width, _sizeBasePaymentButton.Height) { CurrentButtonOid = xpoCheck.Oid, Sensitive = !xpoCheck.Disabled };
                TouchButtonIconWithText buttonMB = new TouchButtonIconWithText("touchButtonMB_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoMB.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoMB.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBasePaymentButton.Width, _sizeBasePaymentButton.Height) { CurrentButtonOid = xpoMB.Oid, Sensitive = !xpoMB.Disabled };
                TouchButtonIconWithText buttonCreditCard = new TouchButtonIconWithText("touchButtonCreditCard_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoCreditCard.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoCreditCard.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBasePaymentButton.Width, _sizeBasePaymentButton.Height) { CurrentButtonOid = xpoCreditCard.Oid, Sensitive = !xpoCreditCard.Disabled };
                /* IN009142 */
                TouchButtonIconWithText buttonDebitCard = new TouchButtonIconWithText("touchButtonDebitCard_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoDebitCard.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoDebitCard.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBasePaymentButton.Width, _sizeBasePaymentButton.Height) { CurrentButtonOid = xpoDebitCard.Oid, Sensitive = !xpoDebitCard.Disabled };
                TouchButtonIconWithText buttonVisa = new TouchButtonIconWithText("touchButtonVisa_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoVisa.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoVisa.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBasePaymentButton.Width, _sizeBasePaymentButton.Height) { CurrentButtonOid = xpoVisa.Oid, Sensitive = !xpoVisa.Disabled };
                TouchButtonIconWithText buttonCurrentAccount = new TouchButtonIconWithText("touchButtonCurrentAccount_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoCurrentAccount.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoCurrentAccount.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoCurrentAccount.Oid, Sensitive = !xpoCurrentAccount.Disabled };
                TouchButtonIconWithText buttonCustomerCard = new TouchButtonIconWithText("touchButtonCustomerCard_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], xpoCustomerCard.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], xpoCustomerCard.ButtonIcon)), _sizeBasePaymentButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoCustomerCard.Oid, Sensitive = !xpoCustomerCard.Disabled };
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
                    (POSSettings.PosPaymentsDialogUseCurrentAccount) ? buttonCurrentAccount : buttonCustomerCard
                    , 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding
                );

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Labels
                Label labelTotal = new Label(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_total_price_to_pay") + ":");
                Label labelDelivery = new Label(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_total_deliver") + ":");
                Label labelChange = new Label(resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_total_change") + ":");
                _labelTotalValue = new Label(SharedUtils.DecimalToStringCurrency(_articleBagFullPayment.TotalFinal))
                {
                    //Total Width
                    WidthRequest = 135
                };
                _labelDeliveryValue = new Label(SharedUtils.DecimalToStringCurrency(0));
                _labelChangeValue = new Label(SharedUtils.DecimalToStringCurrency(0));

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
                eventboxTotalPannel.ModifyBg(StateType.Normal,colorPosPaymentsDialogTotalPannelBackground.ToGdkColor());
                eventboxTotalPannel.Add(tableTotalPannel);

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Customer Name
                CriteriaOperator criteriaOperatorCustomerName = null;
				/* IN009202 */
                _entryBoxSelectCustomerName = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(_sourceWindow, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_customer"), "Name", "Name", null, criteriaOperatorCustomerName, KeyboardMode.Alfa, SharedSettings.RegexAlfaNumericPlus, false);
                _entryBoxSelectCustomerName.ClosePopup += delegate
                {
                    //IN009284 POS - Pagamento conta-corrente - Cliente por defeito 
                    if (_processFinanceDocumentParameter != null)
                    {
                        GetCustomerDetails("Oid", _processFinanceDocumentParameter.Customer.ToString());
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
                _entryBoxCustomerDiscount = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_discount"), KeyboardMode.Alfa, SharedSettings.RegexPercentage, true);
                _entryBoxCustomerDiscount.EntryValidation.Text = SharedUtils.DecimalToString(0.0m);
                _entryBoxCustomerDiscount.EntryValidation.Sensitive = false;
                _entryBoxCustomerDiscount.EntryValidation.Changed += _entryBoxCustomerDiscount_Changed;
                _entryBoxCustomerDiscount.EntryValidation.FocusOutEvent += delegate
                {
                    _entryBoxCustomerDiscount.EntryValidation.Text = SharedUtils.StringToDecimalAndToStringAgain(_entryBoxCustomerDiscount.EntryValidation.Text);
                };

                //Address
                _entryBoxCustomerAddress = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_address"), KeyboardMode.Alfa, SharedSettings.RegexAlfaNumericPlus, false);/* IN009253 */
                _entryBoxCustomerAddress.EntryValidation.Changed += delegate { Validate(); };

                //Locality
                _entryBoxCustomerLocality = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_locality"), KeyboardMode.Alfa, SharedSettings.RegexAlfaNumericPlus, false);/* IN009253 */
                _entryBoxCustomerLocality.EntryValidation.Changed += delegate { Validate(); };

                //ZipCode
                _entryBoxCustomerZipCode = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_zipcode"), KeyboardMode.Alfa, DataLayerSettings.ConfigurationSystemCountry.RegExZipCode, false);
                _entryBoxCustomerZipCode.WidthRequest = 150;
                _entryBoxCustomerZipCode.EntryValidation.Changed += delegate { Validate(); };
                
                //City
                _entryBoxCustomerCity = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_city"), KeyboardMode.Alfa, SharedSettings.RegexAlfaNumericPlus, false);/* IN009253 */
                _entryBoxCustomerCity.WidthRequest = 200;
                _entryBoxCustomerCity.EntryValidation.Changed += delegate { Validate(); };

                //Country
                CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (RegExFiscalNumber IS NOT NULL AND RegExZipCode IS NOT NULL)");
                _entryBoxSelectCustomerCountry = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(pSourceWindow, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_country"), "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, SharedSettings.RegexGuid, true);
                _entryBoxSelectCustomerCountry.WidthRequest = 235;
                //Extra Protection to prevent Customer without Country
                if (_entryBoxSelectCustomerCountry.Value != null) _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                _entryBoxSelectCustomerCountry.EntryValidation.IsEditable = false;
                _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = false;
                _entryBoxSelectCustomerCountry.ClosePopup += delegate
                {
                    _selectedCountry = _entryBoxSelectCustomerCountry.Value;
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
                _entryBoxSelectCustomerFiscalNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(_sourceWindow, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"), "FiscalNumber", "FiscalNumber", null, criteriaOperatorFiscalNumber, KeyboardMode.AlfaNumeric, _intialValueConfigurationCountry.RegExFiscalNumber, false);
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Changed += _entryBoxSelectCustomerFiscalNumber_Changed;

                //CardNumber
                CriteriaOperator criteriaOperatorCardNumber = null;//Now Criteria is assigned in ApplyCriteriaToCustomerInputs();
                _entryBoxSelectCustomerCardNumber = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(_sourceWindow, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_card_number"), "CardNumber", "CardNumber", null, criteriaOperatorCardNumber, KeyboardMode.AlfaNumeric, SharedSettings.RegexAlfaNumericExtended, false);
                _entryBoxSelectCustomerCardNumber.ClosePopup += delegate
                {
                    if (_entryBoxSelectCustomerCardNumber.EntryValidation.Validated) GetCustomerDetails("CardNumber", _entryBoxSelectCustomerCardNumber.EntryValidation.Text);
                    Validate();
                };

                //Notes
                _entryBoxCustomerNotes = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_notes"), KeyboardMode.Alfa, SharedSettings.RegexAlfaNumericExtended, false);
                _entryBoxCustomerNotes.EntryValidation.Changed += delegate { Validate(); };

                //Fill Dialog Inputs with Defaults FinalConsumerEntity Values
                if (_processFinanceDocumentParameter == null)
                {
                    //If ProcessFinanceDocumentParameter is not null fill Dialog with value from ProcessFinanceDocumentParameter, implemented for SplitPayments
                    GetCustomerDetails("Oid", SharedSettings.XpoOidDocumentFinanceMasterFinalConsumerEntity.ToString());
                }
                //Fill Dialog Inputs with Stored Values, ex when we Work with SplitPayments
                else
                {
                    //Apply Default Customer Entity
                    GetCustomerDetails("Oid", _processFinanceDocumentParameter.Customer.ToString());

                    //Assign Totasl and Discounts Values
                    _totalDelivery = _processFinanceDocumentParameter.TotalDelivery;
                    _totalChange = _processFinanceDocumentParameter.TotalChange;
                    _discountGlobal = _processFinanceDocumentParameter.ArticleBag.DiscountGlobal;
                    // Update Visual Components
                    _labelDeliveryValue.Text = SharedUtils.DecimalToStringCurrency(_totalDelivery);
                    _labelChangeValue.Text = SharedUtils.DecimalToStringCurrency(_totalChange);
                    // Selects
                    _selectedCustomer = (erp_customer)DataLayerUtils.GetXPGuidObject(typeof(erp_customer), _processFinanceDocumentParameter.Customer);
                    _selectedCountry = _selectedCustomer.Country;
                    // PaymentMethod
                    _selectedPaymentMethod = (fin_configurationpaymentmethod)DataLayerUtils.GetXPGuidObject(typeof(fin_configurationpaymentmethod), _processFinanceDocumentParameter.PaymentMethod);
                    // Restore Selected Payment Method, require to associate button reference to selectedPaymentMethodButton
                    if (!string.IsNullOrEmpty(pSelectedPaymentMethodButtonName)) {
                        switch (pSelectedPaymentMethodButtonName)
                        {
                            case "touchButtonMoney_Green" :
                                _selectedPaymentMethodButton = buttonMoney;
                                break;
                            case "touchButtonCheck_Green" :
                                _selectedPaymentMethodButton = buttonCheck;
                                break;
                            case "touchButtonMB_Green" :
                                _selectedPaymentMethodButton = buttonMB;
                                break;
                            case "touchButtonCreditCard_Green" :
                                _selectedPaymentMethodButton = buttonCreditCard;
                                break;
                            /* IN009142 */
                            case "touchButtonDebitCard_Green" :
                                _selectedPaymentMethodButton = buttonDebitCard;
                                break;
                            case "touchButtonVisa_Green":
                                _selectedPaymentMethodButton = buttonVisa;
                                break;
                            case "touchButtonCurrentAccount_Green" :
                                _selectedPaymentMethodButton = buttonCurrentAccount;
                                break;
                            case "touchButtonCustomerCard_Green" :
                                _selectedPaymentMethodButton = buttonCustomerCard;
                                break;
                        }

                        //Assign Payment Method after have Reference
                        AssignPaymentMethod(_selectedPaymentMethodButton);
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
                _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
                _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
                _buttonClearCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_button_label_payment_dialog_clear_client"), fileIconClearCustomer);
                _buttonNewCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_button_label_new_client"), fileIconNewCustomer);
                _buttonFullPayment = ActionAreaButton.FactoryGetDialogButtonType("touchButtonFullPayment_DialogActionArea", resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_button_label_payment_dialog_full_payment"), fileIconFullPayment);
                _buttonPartialPayment = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea", resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_button_label_payment_dialog_partial_payment"), fileIconPartialPayment);
                _buttonCurrentAccount = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea", resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type_title_ft"), fileIconCurrentAccount);
                // Enable if has selectedPaymentMethod defined, ex when working with SplitPayments
                _buttonOk.Sensitive = (_selectedPaymentMethod != null);
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
                this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, vboxContent, actionAreaButtons);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}