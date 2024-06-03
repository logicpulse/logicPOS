using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using System;
using System.Drawing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosPayInvoicesDialog : PosBaseDialog
    {
        //Non UI
        private readonly bool _debug = false;
        //Total amount to pay for selected documents, never change
        private readonly decimal _paymentAmountTotal;
        private decimal _paymentAmountEntry;

        public decimal PayedAmount { get; private set; }
        private readonly int _noOfInvoices;
        private bool _validated;
        private decimal _diference;
        //Default is always 1, same has default currency
        private decimal _exchangeRate = 1;
        //UI
        private Fixed _fixedContent;
        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;

        public XPOEntryBoxSelectRecordValidation<fin_configurationpaymentmethod, TreeViewConfigurationPaymentMethod> EntryBoxSelectConfigurationPaymentMethod { get; private set; }

        public XPOEntryBoxSelectRecordValidation<cfg_configurationcurrency, TreeViewConfigurationCurrency> EntryBoxSelectConfigurationCurrency { get; private set; }

        public EntryBoxValidation EntryPaymentAmount { get; private set; }

        public EntryBoxValidation EntryBoxPaymentDate { get; private set; }

        public EntryBoxValidation EntryBoxDocumentPaymentNotes { get; private set; }

        public PosPayInvoicesDialog(Window pSourceWindow, DialogFlags pDialogFlags, decimal pPaymentAmountTotal, int pNoOfInvoices)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _paymentAmountTotal = pPaymentAmountTotal;
            _noOfInvoices = pNoOfInvoices;

            //Init Local Vars
            string windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_pay_invoices");
            _windowSize = new Size(480, 444);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_pay_invoice.png";

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            _buttonOk.Sensitive = false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

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
            fin_configurationpaymentmethod initialValueConfigurationPaymentMethod = XPOHelper.GetEntityById<fin_configurationpaymentmethod>(POSSettings.XpoOidConfigurationPaymentMethodDefaultInvoicePaymentMethod);
            cfg_configurationcurrency intialValueConfigurationCurrency = XPOSettings.ConfigurationSystemCurrency;
            string initialPaymentDate = XPOHelper.CurrentDateTimeAtomic().ToString(CultureSettings.DateTimeFormat);

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
            CriteriaOperator criteriaOperatorConfigurationPaymentMethod = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1)  AND Oid <> '{0}' AND {1}", InvoiceSettings.XpoOidConfigurationPaymentMethodCurrentAccount.ToString(), filterValidPaymentMethod));
            EntryBoxSelectConfigurationPaymentMethod = new XPOEntryBoxSelectRecordValidation<fin_configurationpaymentmethod, TreeViewConfigurationPaymentMethod>(_sourceWindow, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_payment_method"), "Designation", "Oid", initialValueConfigurationPaymentMethod, criteriaOperatorConfigurationPaymentMethod, LogicPOS.Utility.RegexUtils.RegexGuid, true);
            EntryBoxSelectConfigurationPaymentMethod.EntryValidation.Changed += delegate { Validate(); };
            EntryBoxSelectConfigurationPaymentMethod.EntryValidation.IsEditable = false;

            //ConfigurationCurrency
            CriteriaOperator criteriaOperatorConfigurationCurrency = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (ExchangeRate IS NOT NULL OR Oid = '{0}')", XPOSettings.ConfigurationSystemCurrency.Oid.ToString()));
            EntryBoxSelectConfigurationCurrency = new XPOEntryBoxSelectRecordValidation<cfg_configurationcurrency, TreeViewConfigurationCurrency>(_sourceWindow, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_currency"), "Designation", "Oid", intialValueConfigurationCurrency, criteriaOperatorConfigurationCurrency, LogicPOS.Utility.RegexUtils.RegexGuid, false);
            EntryBoxSelectConfigurationCurrency.EntryValidation.Changed += _entryBoxSelectConfigurationCurrency_Changed;
            EntryBoxSelectConfigurationCurrency.EntryValidation.IsEditable = false;

            //PaymentAmount
            /* IN009183 */
            EntryPaymentAmount = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_deliver"), KeyboardMode.Numeric, LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZeroFinancial, true);
            EntryPaymentAmount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_paymentAmountTotal);
            EntryPaymentAmount.EntryValidation.Validate();
            EntryPaymentAmount.EntryValidation.Changed += delegate
            {
                Validate();
                UpdateTitleBar();
            };

            //PaymentDate
            EntryBoxPaymentDate = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_date"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexDateTime, true);
            EntryBoxPaymentDate.EntryValidation.Text = initialPaymentDate;
            EntryBoxPaymentDate.EntryValidation.Validate();
            EntryBoxPaymentDate.EntryValidation.Changed += delegate { Validate(); };

            //PaymentNotes
            EntryBoxDocumentPaymentNotes = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_notes"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            EntryBoxDocumentPaymentNotes.EntryValidation.Changed += delegate { Validate(); };

            //Pack VBOX
            VBox vbox = new VBox(false, 0);
            vbox.PackStart(EntryBoxSelectConfigurationPaymentMethod, true, true, 0);
            vbox.PackStart(EntryBoxSelectConfigurationCurrency, false, false, 0);
            vbox.PackStart(EntryPaymentAmount, false, false, 0);
            vbox.PackStart(EntryBoxPaymentDate, false, false, 0);
            vbox.PackStart(EntryBoxDocumentPaymentNotes, false, false, 0);
            vbox.PackStart(EntryBoxDocumentPaymentNotes, false, false, 0);
            vbox.WidthRequest = _windowSize.Width - 14;

            //Put in FinishContent
            _fixedContent = new Fixed();
            _fixedContent.Put(vbox, 0, 0);
        }

        public void Validate()
        {
            //Settings
            int decimalRoundTo = CultureSettings.DecimalRoundTo;

            //Check if Has More than one Invoice and the Input is The Full Payment
            _paymentAmountEntry = LogicPOS.Utility.DataConversionUtils.StringToDecimal(EntryPaymentAmount.EntryValidation.Text);

            //Calc Diference in selected Currency
            _diference = Math.Round((_paymentAmountTotal * _exchangeRate) - _paymentAmountEntry, decimalRoundTo);

            _validated = (
              EntryBoxSelectConfigurationPaymentMethod.EntryValidation.Validated &&
              EntryBoxSelectConfigurationCurrency.EntryValidation.Validated &&
              EntryPaymentAmount.EntryValidation.Validated &&
              EntryBoxPaymentDate.EntryValidation.Validated &&
              EntryBoxDocumentPaymentNotes.EntryValidation.Validated &&
              (_diference >= 0)
             );

            //Update Payed amount in default currency, must divided by ExchangeRate, inputs are always in selected Currency 
            PayedAmount = _paymentAmountEntry / _exchangeRate;
            if (_debug) _logger.Debug(string.Format("_payedAmount/_paymentAmountTotal: [{0}/{1}]", LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(PayedAmount, XPOSettings.ConfigurationSystemCurrency.Acronym), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(_paymentAmountTotal, XPOSettings.ConfigurationSystemCurrency.Acronym)));

            //Block Change of Currency to prevent conversion problems
            EntryBoxSelectConfigurationCurrency.EntryValidation.Sensitive = EntryPaymentAmount.EntryValidation.Validated;
            EntryBoxSelectConfigurationCurrency.ButtonSelectValue.Sensitive = EntryPaymentAmount.EntryValidation.Validated;

            /* IN009183 */
            GtkUtils.UpdateWidgetColorsAfterValidation(EntryPaymentAmount.EntryValidation, (_validated && _diference >= 0), EntryPaymentAmount.Label);
            //_entryPaymentAmount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_paymentAmountEntry, LogicPOS.Settings.CultureSettings.CurrentCulture, SettingsApp.DecimalFormat); //LogicPOS.Utility.DataConversionUtils.DecimalToString(_paymentAmountEntry);
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
                        paymentPercentage = (PayedAmount * 100) / _paymentAmountTotal;
                    }

                    WindowTitle = string.Format(
                        "{0}: {1} / {2}{3}",
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_pay_invoices"),
                        LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(_diference, EntryBoxSelectConfigurationCurrency.Value.Acronym),
                        LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(_paymentAmountTotal * _exchangeRate, EntryBoxSelectConfigurationCurrency.Value.Acronym),
                        //Show only if above 100%
                        (paymentPercentage < 100) ? string.Format(" / {0}%", LogicPOS.Utility.DataConversionUtils.DecimalToString(paymentPercentage)) : string.Empty
                     );
                }
                else
                {
                    WindowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_pay_invoices");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void _entryBoxSelectConfigurationCurrency_Changed(object sender, EventArgs e)
        {
            //Update ExchangeRate Reference
            _exchangeRate = EntryBoxSelectConfigurationCurrency.Value.ExchangeRate;
            //Always Update Entry Value too paymentAmountTotal to prevent round values, this way when we change currency, we always assign to default paymentAmountTotal value 
            EntryPaymentAmount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(_paymentAmountTotal * _exchangeRate);
            //Call Validate
            Validate();
            //Call Update Title Bar
            UpdateTitleBar();
        }
    }
}
