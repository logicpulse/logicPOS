using DevExpress.Data.Filtering;
using Gtk;
using logicpos.financial;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.shared;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosPayInvoicesDialog : PosBaseDialog
    {
        //Non UI
        private bool _debug = false;
        //Total amount to pay for selected documents, never change
        private decimal _paymentAmountTotal;
        private decimal _paymentAmountEntry;
        //Value in Default Currency [EUR], used has return valur for dialog in PayInvoices( method
        private decimal _payedAmount;
        public decimal PayedAmount { get { return _payedAmount; } }
        private int _noOfInvoices;
        private bool _validated;
        private decimal _diference;
        //Default is always 1, same has default currency
        private decimal _exchangeRate = 1;
        //UI
        private Fixed _fixedContent;
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        //UI Components
        private XPOEntryBoxSelectRecordValidation<fin_configurationpaymentmethod, TreeViewConfigurationPaymentMethod> _entryBoxSelectConfigurationPaymentMethod;
        public XPOEntryBoxSelectRecordValidation<fin_configurationpaymentmethod, TreeViewConfigurationPaymentMethod> EntryBoxSelectConfigurationPaymentMethod
        {
            get { return _entryBoxSelectConfigurationPaymentMethod; }
        }

        private XPOEntryBoxSelectRecordValidation<cfg_configurationcurrency, TreeViewConfigurationCurrency> _entryBoxSelectConfigurationCurrency;
        public XPOEntryBoxSelectRecordValidation<cfg_configurationcurrency, TreeViewConfigurationCurrency> EntryBoxSelectConfigurationCurrency
        {
            get { return _entryBoxSelectConfigurationCurrency; }
        }

        private EntryBoxValidation _entryPaymentAmount;
        public EntryBoxValidation EntryPaymentAmount
        {
            get { return _entryPaymentAmount; }
        }

        private EntryBoxValidation _entryBoxPaymentDate;
        public EntryBoxValidation EntryBoxPaymentDate
        {
            get { return _entryBoxPaymentDate; }
        }

        private EntryBoxValidation _entryBoxDocumentPaymentNotes;
        public EntryBoxValidation EntryBoxDocumentPaymentNotes
        {
            get { return _entryBoxDocumentPaymentNotes; }
        }

        public PosPayInvoicesDialog(Window pSourceWindow, DialogFlags pDialogFlags, decimal pPaymentAmountTotal, int pNoOfInvoices)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _paymentAmountTotal = pPaymentAmountTotal;
            _noOfInvoices = pNoOfInvoices;

            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_pay_invoices");
            _windowSize = new Size(480, 444);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_pay_invoice.png");

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
			_buttonOk.Sensitive = false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Init Content
            InitUI();

            //Start Validated
            Validate();

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, _fixedContent, actionAreaButtons);
        }

        private void InitUI()
        {
            //Initial Values
            fin_configurationpaymentmethod initialValueConfigurationPaymentMethod = (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObject(typeof(fin_configurationpaymentmethod), SettingsApp.XpoOidConfigurationPaymentMethodDefaultInvoicePaymentMethod);
            cfg_configurationcurrency intialValueConfigurationCurrency = SettingsApp.ConfigurationSystemCurrency;
            string initialPaymentDate = FrameworkUtils.CurrentDateTimeAtomic().ToString(SettingsApp.DateTimeFormat);

            /* IN009142
            * 
            * Valid Payment Methods for "Liquidar Faturas":
            * 
            * - CREDIT_CARD
            * - DEBIT_CARD
            * - BANK_CHECK
            * - CASH_MACHINE
            * - MONEY
            * - BANK_TRANSFER
            */
            string filterValidPaymentMethod = @"
(
    Token = 'CREDIT_CARD' OR
    Token = 'DEBIT_CARD' OR
    Token = 'BANK_CHECK' OR 
    Token = 'CASH_MACHINE' OR 
    Token = 'MONEY' OR 
    Token = 'BANK_TRANSFER'
)";
            CriteriaOperator criteriaOperatorConfigurationPaymentMethod = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1)  AND Oid <> '{0}' AND {1}", SettingsApp.XpoOidConfigurationPaymentMethodCurrentAccount.ToString(), filterValidPaymentMethod));
            _entryBoxSelectConfigurationPaymentMethod = new XPOEntryBoxSelectRecordValidation<fin_configurationpaymentmethod, TreeViewConfigurationPaymentMethod>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_payment_method"), "Designation", "Oid", initialValueConfigurationPaymentMethod, criteriaOperatorConfigurationPaymentMethod, SettingsApp.RegexGuid, true);
            _entryBoxSelectConfigurationPaymentMethod.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxSelectConfigurationPaymentMethod.EntryValidation.IsEditable = false;

            //ConfigurationCurrency
            CriteriaOperator criteriaOperatorConfigurationCurrency = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (ExchangeRate IS NOT NULL OR Oid = '{0}')", SettingsApp.ConfigurationSystemCurrency.Oid.ToString()));
            _entryBoxSelectConfigurationCurrency = new XPOEntryBoxSelectRecordValidation<cfg_configurationcurrency, TreeViewConfigurationCurrency>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_currency"), "Designation", "Oid", intialValueConfigurationCurrency, criteriaOperatorConfigurationCurrency, SettingsApp.RegexGuid, false);
            _entryBoxSelectConfigurationCurrency.EntryValidation.Changed += _entryBoxSelectConfigurationCurrency_Changed;
            _entryBoxSelectConfigurationCurrency.EntryValidation.IsEditable = false;

            //PaymentAmount
            /* IN009183 */
            _entryPaymentAmount = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_deliver"), KeyboardMode.Numeric, SettingsApp.RegexDecimalGreaterEqualThanZeroFinancial, true);
            _entryPaymentAmount.EntryValidation.Text = FrameworkUtils.DecimalToString(_paymentAmountTotal);
            _entryPaymentAmount.EntryValidation.Validate();
            _entryPaymentAmount.EntryValidation.Changed += delegate {
                Validate();
                UpdateTitleBar(); 
            };

            //PaymentDate
            _entryBoxPaymentDate = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), KeyboardMode.Alfa, SettingsApp.RegexDateTime, true);
            _entryBoxPaymentDate.EntryValidation.Text = initialPaymentDate;
            _entryBoxPaymentDate.EntryValidation.Validate();
            _entryBoxPaymentDate.EntryValidation.Changed += delegate { Validate(); };

            //PaymentNotes
            _entryBoxDocumentPaymentNotes = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            _entryBoxDocumentPaymentNotes.EntryValidation.Changed += delegate { Validate(); };

            //Pack VBOX
            VBox vbox = new VBox(false, 0);
            vbox.PackStart(_entryBoxSelectConfigurationPaymentMethod, true, true, 0);
            vbox.PackStart(_entryBoxSelectConfigurationCurrency, false, false, 0);
            vbox.PackStart(_entryPaymentAmount, false, false, 0);
            vbox.PackStart(_entryBoxPaymentDate, false, false, 0);
            vbox.PackStart(_entryBoxDocumentPaymentNotes, false, false, 0);
            vbox.PackStart(_entryBoxDocumentPaymentNotes, false, false, 0);
            vbox.WidthRequest = _windowSize.Width - 14;

            //Put in FinishContent
            _fixedContent = new Fixed();
            _fixedContent.Put(vbox, 0, 0);
        }

        public void Validate()
        {
            //Settings
            int decimalRoundTo = SettingsApp.DecimalRoundTo;

            //Check if Has More than one Invoice and the Input is The Full Payment
            _paymentAmountEntry = FrameworkUtils.StringToDecimal(_entryPaymentAmount.EntryValidation.Text);

            //Calc Diference in selected Currency
            _diference = Math.Round((_paymentAmountTotal * _exchangeRate) - _paymentAmountEntry, decimalRoundTo);

            _validated = (
              _entryBoxSelectConfigurationPaymentMethod.EntryValidation.Validated &&
              _entryBoxSelectConfigurationCurrency.EntryValidation.Validated &&
              _entryPaymentAmount.EntryValidation.Validated &&
              _entryBoxPaymentDate.EntryValidation.Validated &&
              _entryBoxDocumentPaymentNotes.EntryValidation.Validated &&
              (_diference >= 0)
             );

            //Update Payed amount in default currency, must divided by ExchangeRate, inputs are always in selected Currency 
            _payedAmount = _paymentAmountEntry / _exchangeRate;
            if (_debug) _log.Debug(string.Format("_payedAmount/_paymentAmountTotal: [{0}/{1}]", FrameworkUtils.DecimalToStringCurrency(_payedAmount), FrameworkUtils.DecimalToStringCurrency(_paymentAmountTotal)));

            //Block Change of Currency to prevent conversion problems
            _entryBoxSelectConfigurationCurrency.EntryValidation.Sensitive = _entryPaymentAmount.EntryValidation.Validated;
            _entryBoxSelectConfigurationCurrency.ButtonSelectValue.Sensitive = _entryPaymentAmount.EntryValidation.Validated;

            /* IN009183 */
            Utils.ValidateUpdateColors(_entryPaymentAmount.EntryValidation, _entryPaymentAmount.Label, (_validated && _diference >= 0));
            //_entryPaymentAmount.EntryValidation.Text = FrameworkUtils.DecimalToString(_paymentAmountEntry, GlobalFramework.CurrentCulture, SettingsApp.DecimalFormat); //FrameworkUtils.DecimalToString(_paymentAmountEntry);
            // _paymentAmountEntry = Math.Round(_paymentAmountEntry, decimalRoundTo);

            _buttonOk.Sensitive = _validated;
        }

        private void UpdateTitleBar()
        {
            try
            {
                decimal paymentPercentage = 0.0m;

                if (_diference < 0 || _diference > 0)
                {
                    //Calculate in Default Currency [EUR]
                    if (_paymentAmountTotal > 0)
                    {
                        paymentPercentage = (_payedAmount * 100) / _paymentAmountTotal;
                    }

                    WindowTitle = string.Format(
                        "{0}: {1} / {2}{3}",
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_pay_invoices"),
                        FrameworkUtils.DecimalToStringCurrency(_diference, _entryBoxSelectConfigurationCurrency.Value.Acronym),
                        FrameworkUtils.DecimalToStringCurrency(_paymentAmountTotal * _exchangeRate, _entryBoxSelectConfigurationCurrency.Value.Acronym),
                        //Show only if above 100%
                        (paymentPercentage < 100) ? string.Format(" / {0}%", FrameworkUtils.DecimalToString(paymentPercentage)) : string.Empty
                     );
                }
                else
                {
                    WindowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_pay_invoices");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        void _entryBoxSelectConfigurationCurrency_Changed(object sender, EventArgs e)
        {
            //Update ExchangeRate Reference
            _exchangeRate = _entryBoxSelectConfigurationCurrency.Value.ExchangeRate;
            //Always Update Entry Value too paymentAmountTotal to prevent round values, this way when we change currency, we always assign to default paymentAmountTotal value 
            _entryPaymentAmount.EntryValidation.Text = FrameworkUtils.DecimalToString(_paymentAmountTotal * _exchangeRate);
            //Call Validate
            Validate();
            //Call Update Title Bar
            UpdateTitleBar();
        }
    }
}
