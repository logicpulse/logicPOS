using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.shared.Classes.Finance;
using System;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosPaymentsDialog : PosBaseDialog
    {
        //Settings
        private Color _colorEntryValidationValidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidFont"]);
        private Color _colorEntryValidationInvalidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationInvalidFont"]);
        //Usefull to block .Change events when we change Entry.Text from code, and prevent to recursivly call Change Events
        private bool _enableGetCustomerDetails = true;
        //PartialPayments Stuff
        private PosSelectRecordDialog<DataTable, DataRow, TreeViewPartialPayment> _dialogPartialPayment;
        private decimal _totalPartialPaymentItems = 0;
        //Default DocumentType (FS)
        private Guid _processDocumentType = SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice;
        //ResponseType (Above 10)
        private ResponseType _responseTypeClearCustomer = (ResponseType)11;
        private ResponseType _responseTypeFullPayment = (ResponseType)12;
        private ResponseType _responseTypePartialPayment = (ResponseType)13;
        //UI
        private Label _labelTotalValue;
        private Label _labelDeliveryValue;
        private Label _labelChangeValue;
        //UI EntryBox
        private XPOEntryBoxSelectRecordValidation<ERP_Customer, TreeViewCustomer> _entryBoxSelectCustomerFiscalNumber;
        private XPOEntryBoxSelectRecordValidation<ERP_Customer, TreeViewCustomer> _entryBoxSelectCustomerCardNumber;
        private XPOEntryBoxSelectRecordValidation<ERP_Customer, TreeViewCustomer> _entryBoxSelectCustomerName;
        private XPOEntryBoxSelectRecordValidation<CFG_ConfigurationCountry, TreeViewConfigurationCountry> _entryBoxSelectCustomerCountry;
        private EntryBoxValidation _entryBoxCustomerDiscount;
        private EntryBoxValidation _entryBoxCustomerAddress;
        private EntryBoxValidation _entryBoxCustomerLocality;
        private EntryBoxValidation _entryBoxCustomerZipCode;
        private EntryBoxValidation _entryBoxCustomerCity;
        private EntryBoxValidation _entryBoxCustomerNotes;
        //Buttons
        private TouchButtonBase _selectedPaymentMethodButton;
        //ActionArea
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        private TouchButtonIconWithText _buttonClearCustomer;
        private TouchButtonIconWithText _buttonFullPayment;
        private TouchButtonIconWithText _buttonPartialPayment;
        //Default Objects
        private CFG_ConfigurationCountry _intialValueConfigurationCountry;
        //Store Partial Payment Enabled/Disabled
        private bool _partialPaymentEnabled = false;
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
        private FIN_ConfigurationPaymentMethod _selectedPaymentMethod;
        public FIN_ConfigurationPaymentMethod PaymentMethod
        {
            get { return _selectedPaymentMethod; }
            set { _selectedPaymentMethod = value; }
        }
        private ERP_Customer _selectedCustomer;
        public ERP_Customer Customer
        {
            get { return _selectedCustomer; }
            set { _selectedCustomer = value; }
        }
        private CFG_ConfigurationCountry _selectedCountry;
        public CFG_ConfigurationCountry Country
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

        //Constructors
        public PosPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag pArticleBag)
            : this(pSourceWindow, pDialogFlags, pArticleBag, true, true) { }

        public PosPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag pArticleBag, bool pEnablePartialPaymentButtons)
            : this(pSourceWindow, pDialogFlags, pArticleBag, pEnablePartialPaymentButtons, true) { }

        public PosPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag pArticleBag, bool pEnablePartialPaymentButtons, bool pEnableCurrentAccountButton)
            : base(pSourceWindow, pDialogFlags, false)
        {
            try
            {
                //Init Local Vars
                _sourceWindow = pSourceWindow;
                string windowTitle = Resx.window_title_dialog_payments;
                //TODO:THEME
                Size windowSize = new Size(598, 620);
                string fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_payments.png");

                //Parameters
                _articleBagFullPayment = pArticleBag;
                bool enablePartialPaymentButtons = pEnablePartialPaymentButtons;
                bool enableCurrentAccountButton = pEnableCurrentAccountButton;
                if (enablePartialPaymentButtons) enablePartialPaymentButtons = (_articleBagFullPayment.TotalQuantity > 1) ? true : false;
                //Files
                string fileIconClearCustomer = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_nav_delete.png");
                string fileIconFullPayment = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_payment_full.png");
                string fileIconPartialPayment = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_payment_partial.png");
                //Colors
                Color colorPosPaymentsDialogTotalPannelBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosPaymentsDialogTotalPannelBackground"]);
                //Objects
                _intialValueConfigurationCountry = SettingsApp.ConfigurationSystemCountry;

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Payment Buttons
                //Get Custom Select Data
                string executeSql = @"SELECT Oid, Token, ResourceString FROM fin_configurationpaymentmethod ORDER BY Ord;";
                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(executeSql);
                //Get Required XpObjects from Selected Data
                FIN_ConfigurationPaymentMethod xpoMoney = (FIN_ConfigurationPaymentMethod)xPSelectData.GetXPGuidObjectFromField(typeof(FIN_ConfigurationPaymentMethod), "Token", "MONEY");
                FIN_ConfigurationPaymentMethod xpoCheck = (FIN_ConfigurationPaymentMethod)xPSelectData.GetXPGuidObjectFromField(typeof(FIN_ConfigurationPaymentMethod), "Token", "BANK_CHECK");
                FIN_ConfigurationPaymentMethod xpoMB = (FIN_ConfigurationPaymentMethod)xPSelectData.GetXPGuidObjectFromField(typeof(FIN_ConfigurationPaymentMethod), "Token", "CASH_MACHINE");
                FIN_ConfigurationPaymentMethod xpoCreditCard = (FIN_ConfigurationPaymentMethod)xPSelectData.GetXPGuidObjectFromField(typeof(FIN_ConfigurationPaymentMethod), "Token", "CREDIT_CARD");
                FIN_ConfigurationPaymentMethod xpoVisa = (FIN_ConfigurationPaymentMethod)xPSelectData.GetXPGuidObjectFromField(typeof(FIN_ConfigurationPaymentMethod), "Token", "VISA");
                FIN_ConfigurationPaymentMethod xpoCurrentAccount = (FIN_ConfigurationPaymentMethod)xPSelectData.GetXPGuidObjectFromField(typeof(FIN_ConfigurationPaymentMethod), "Token", "CURRENT_ACCOUNT");
                FIN_ConfigurationPaymentMethod xpoCustomerCard = (FIN_ConfigurationPaymentMethod)xPSelectData.GetXPGuidObjectFromField(typeof(FIN_ConfigurationPaymentMethod), "Token", "CUSTOMER_CARD");

                //Instantiate Buttons
                TouchButtonIconWithText buttonMoney = new TouchButtonIconWithText("touchButtonMoney_Green", _colorBaseDialogDefaultButtonBackground, Resx.ResourceManager.GetString(xpoMoney.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], xpoMoney.ButtonIcon)), _sizeBaseDialogDefaultButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoMoney.Oid, Sensitive = (xpoMoney.Disabled) ? false : true };
                TouchButtonIconWithText buttonCheck = new TouchButtonIconWithText("touchButtonCheck_Green", _colorBaseDialogDefaultButtonBackground, Resx.ResourceManager.GetString(xpoCheck.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], xpoCheck.ButtonIcon)), _sizeBaseDialogDefaultButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoCheck.Oid, Sensitive = (xpoCheck.Disabled) ? false : true };
                TouchButtonIconWithText buttonMB = new TouchButtonIconWithText("touchButtonMB_Green", _colorBaseDialogDefaultButtonBackground, Resx.ResourceManager.GetString(xpoMB.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], xpoMB.ButtonIcon)), _sizeBaseDialogDefaultButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoMB.Oid, Sensitive = (xpoMB.Disabled) ? false : true };
                TouchButtonIconWithText buttonCreditCard = new TouchButtonIconWithText("touchButtonCreditCard_Green", _colorBaseDialogDefaultButtonBackground, Resx.ResourceManager.GetString(xpoCreditCard.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], xpoCreditCard.ButtonIcon)), _sizeBaseDialogDefaultButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoCreditCard.Oid, Sensitive = (xpoCreditCard.Disabled) ? false : true };
                TouchButtonIconWithText buttonVisa = new TouchButtonIconWithText("touchButtonVisa_Green", _colorBaseDialogDefaultButtonBackground, Resx.ResourceManager.GetString(xpoVisa.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], xpoVisa.ButtonIcon)), _sizeBaseDialogDefaultButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoVisa.Oid, Sensitive = (xpoVisa.Disabled) ? false : true };
                TouchButtonIconWithText buttonCurrentAccount = new TouchButtonIconWithText("touchButtonCurrentAccount_Green", _colorBaseDialogDefaultButtonBackground, Resx.ResourceManager.GetString(xpoCurrentAccount.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], xpoCurrentAccount.ButtonIcon)), _sizeBaseDialogDefaultButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoCurrentAccount.Oid, Sensitive = (xpoCurrentAccount.Disabled) ? false : true };
                TouchButtonIconWithText buttonCustomerCard = new TouchButtonIconWithText("touchButtonCustomerCard_Green", _colorBaseDialogDefaultButtonBackground, Resx.ResourceManager.GetString(xpoCustomerCard.ResourceString), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], xpoCustomerCard.ButtonIcon)), _sizeBaseDialogDefaultButtonIcon, _sizeBaseDialogDefaultButton.Width, _sizeBaseDialogDefaultButton.Height) { CurrentButtonOid = xpoCustomerCard.Oid, Sensitive = (xpoCustomerCard.Disabled) ? false : true };
                //Secondary Buttons
                //Events
                buttonMoney.Clicked += buttonMoney_Clicked;
                buttonCheck.Clicked += buttonCheck_Clicked;
                buttonMB.Clicked += buttonMB_Clicked;
                buttonCreditCard.Clicked += buttonCredit_Clicked;
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
                tablePayments.Attach(buttonVisa, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
                //Row 2
                tablePayments.Attach(buttonCheck, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
                tablePayments.Attach(buttonCreditCard, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding);
                if (enableCurrentAccountButton) tablePayments.Attach(
                    (SettingsApp.PosPaymentsDialogUseCurrentAccount) ? buttonCurrentAccount : buttonCustomerCard
                    , 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePaymentsPadding, tablePaymentsPadding
                );

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Labels
                Label labelTotal = new Label(Resx.global_total_price_to_pay + ":");
                Label labelDelivery = new Label(Resx.global_total_deliver + ":");
                Label labelChange = new Label(Resx.global_total_change + ":");
                _labelTotalValue = new Label(FrameworkUtils.DecimalToStringCurrency(_articleBagFullPayment.TotalFinal))
                {
                    //Total Width
                    WidthRequest = 120
                };
                _labelDeliveryValue = new Label(FrameworkUtils.DecimalToStringCurrency(0));
                _labelChangeValue = new Label(FrameworkUtils.DecimalToStringCurrency(0));

                //Colors
                labelTotal.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.FromArgb(101, 137, 171)));
                labelDelivery.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.FromArgb(101, 137, 171)));
                labelChange.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.FromArgb(101, 137, 171)));
                _labelTotalValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.White));
                _labelDeliveryValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.White));
                _labelChangeValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.White));

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
                eventboxTotalPannel.BorderWidth = 3;
                eventboxTotalPannel.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorPosPaymentsDialogTotalPannelBackground));
                eventboxTotalPannel.Add(tableTotalPannel);

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Customer Name
                CriteriaOperator criteriaOperatorCustomerName = null;
                _entryBoxSelectCustomerName = new XPOEntryBoxSelectRecordValidation<ERP_Customer, TreeViewCustomer>(_sourceWindow, Resx.global_customer, "Name", "Name", null, criteriaOperatorCustomerName, KeyboardMode.Alfa, SettingsApp.RegexAlfa, false);
                _entryBoxSelectCustomerName.ClosePopup += delegate
                {
                    GetCustomerDetails("Oid", _entryBoxSelectCustomerName.Value.Oid.ToString());
                    Validate();
                };
                _entryBoxSelectCustomerName.EntryValidation.Changed += _entryBoxSelectCustomerName_Changed;

                //Customer Discount
                _entryBoxCustomerDiscount = new EntryBoxValidation(_sourceWindow, Resx.global_discount, KeyboardMode.Alfa, SettingsApp.RegexPercentage, true);
                _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(0.0m);
                _entryBoxCustomerDiscount.EntryValidation.Sensitive = false;
                _entryBoxCustomerDiscount.EntryValidation.Changed += _entryBoxCustomerDiscount_Changed;
                _entryBoxCustomerDiscount.EntryValidation.FocusOutEvent += delegate
                {
                    _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.StringToDecimalAndToStringAgain(_entryBoxCustomerDiscount.EntryValidation.Text);
                };

                //Address
                _entryBoxCustomerAddress = new EntryBoxValidation(this, Resx.global_address, KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
                _entryBoxCustomerAddress.EntryValidation.Changed += delegate { Validate(); };

                //Locality
                _entryBoxCustomerLocality = new EntryBoxValidation(this, Resx.global_locality, KeyboardMode.Alfa, SettingsApp.RegexAlfa, false);
                _entryBoxCustomerLocality.EntryValidation.Changed += delegate { Validate(); };

                //ZipCode
                _entryBoxCustomerZipCode = new EntryBoxValidation(this, Resx.global_zipcode, KeyboardMode.Alfa, SettingsApp.ConfigurationSystemCountry.RegExZipCode, false);
                _entryBoxCustomerZipCode.WidthRequest = 150;
                _entryBoxCustomerZipCode.EntryValidation.Changed += delegate { Validate(); };

                //City
                _entryBoxCustomerCity = new EntryBoxValidation(this, Resx.global_city, KeyboardMode.Alfa, SettingsApp.RegexAlfa, false);
                _entryBoxCustomerCity.WidthRequest = 200;
                _entryBoxCustomerCity.EntryValidation.Changed += delegate { Validate(); };

                //Country
                CriteriaOperator criteriaOperatorCustomerCountry = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1) AND (RegExFiscalNumber IS NOT NULL)");
                _entryBoxSelectCustomerCountry = new XPOEntryBoxSelectRecordValidation<CFG_ConfigurationCountry, TreeViewConfigurationCountry>(pSourceWindow, Resx.global_country, "Designation", "Oid", _intialValueConfigurationCountry, criteriaOperatorCustomerCountry, SettingsApp.RegexGuid, true);
                _entryBoxSelectCustomerCountry.WidthRequest = 235;
                //Extra Protection to prevent Customer without Country
                if (_entryBoxSelectCustomerCountry.Value != null) _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                _entryBoxSelectCustomerCountry.EntryValidation.IsEditable = false;
                _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = false;
                _entryBoxSelectCustomerCountry.ClosePopup += delegate
                {
                    _selectedCountry = _entryBoxSelectCustomerCountry.Value;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExFiscalNumber;
                    //Clear Customer Fields, Except Country
                    ClearCustomer(false);
                    //Apply Criteria Operators
                    ApplyCriteriaToCustomerInputs();
                    //Call Main Validate
                    Validate();
                };

                //FiscalNumber
                CriteriaOperator criteriaOperatorFiscalNumber = null;
                _entryBoxSelectCustomerFiscalNumber = new XPOEntryBoxSelectRecordValidation<ERP_Customer, TreeViewCustomer>(_sourceWindow, Resx.global_fiscal_number, "FiscalNumber", "FiscalNumber", null, criteriaOperatorFiscalNumber, KeyboardMode.AlfaNumeric, _intialValueConfigurationCountry.RegExFiscalNumber, false);
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Changed += _entryBoxSelectCustomerFiscalNumber_Changed;

                //CardNumber
                CriteriaOperator criteriaOperatorCardNumber = null;//Now Criteria is assigned in ApplyCriteriaToCustomerInputs();
                _entryBoxSelectCustomerCardNumber = new XPOEntryBoxSelectRecordValidation<ERP_Customer, TreeViewCustomer>(_sourceWindow, Resx.global_card_number, "CardNumber", "CardNumber", null, criteriaOperatorCardNumber, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, false);
                _entryBoxSelectCustomerCardNumber.ClosePopup += delegate
                {
                    if (_entryBoxSelectCustomerCardNumber.EntryValidation.Validated) GetCustomerDetails("CardNumber", _entryBoxSelectCustomerCardNumber.EntryValidation.Text);
                    Validate();
                };

                //Notes
                _entryBoxCustomerNotes = new EntryBoxValidation(this, Resx.global_notes, KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
                _entryBoxCustomerNotes.EntryValidation.Changed += delegate { Validate(); };

                //Apply Default Customer Entity
                GetCustomerDetails("Oid", SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity.ToString());

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
                _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(ActionAreaButton.PosBaseDialogButtonType.Ok);
                _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(ActionAreaButton.PosBaseDialogButtonType.Cancel);
                _buttonClearCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", Resx.global_button_label_payment_dialog_clear_client, fileIconClearCustomer);
                _buttonFullPayment = ActionAreaButton.FactoryGetDialogButtonType("touchButtonFullPayment_DialogActionArea", Resx.global_button_label_payment_dialog_full_payment, fileIconFullPayment);
                _buttonPartialPayment = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea", Resx.global_button_label_payment_dialog_partial_payment, fileIconPartialPayment);
                _buttonOk.Sensitive = false;
                _buttonFullPayment.Sensitive = false;

                //ActionArea
                ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
                actionAreaButtons.Add(new ActionAreaButton(_buttonClearCustomer, _responseTypeClearCustomer));
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
                _log.Error(ex.Message, ex);
            }
        }
    }
}