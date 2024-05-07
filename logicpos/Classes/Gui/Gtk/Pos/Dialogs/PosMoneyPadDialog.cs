using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.shared.App;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class MoneyPadResult
    {
        public ResponseType Response { get; set; }

        public decimal Value { get; set; }

        public MoneyPadResult(ResponseType pResponse, decimal pValue)
        {
            Response = pResponse;
            Value = pValue;
        }
    }

    internal class PosMoneyPadDialog : PosBaseDialog
    {
        //UI
        private MoneyPad _moneyPad;
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;

        public decimal Amount { get; set; } = 0.0m;

        public decimal TotalOrder { get; set; } = 0.0m;

        public PosMoneyPadDialog(Window pSourceWindow, DialogFlags pDialogFlags, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle;
            if (pTotalOrder > 0)
            {
                windowTitle = string.Format("{0} - {1} : {2}", CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_dialog_moneypad"), CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_total_table_tickets"), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(pTotalOrder, SharedSettings.ConfigurationSystemCurrency.Acronym));
            }
            else
            {
                windowTitle = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_dialog_moneypad");
            }

            this.InitObject(pSourceWindow, pDialogFlags, windowTitle, pInitialValue, pTotalOrder);
        }

        public PosMoneyPadDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowTitle, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Object
            this.InitObject(pSourceWindow, pDialogFlags, pWindowTitle, pInitialValue, pTotalOrder);
        }

        public void InitObject(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowTitle, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
        {
            Size windowSize = new Size(524, 497);
            string fileDefaultWindowIcon = GeneralSettings.Path["images"] + @"Icons\Windows\icon_window_payments.png";

            //Init MoneyPad
            _moneyPad = new MoneyPad(pSourceWindow, pInitialValue);
            _moneyPad.EntryChanged += _moneyPad_EntryChanged;
            //If pInitialValue defined, Assign it
            Amount = (pInitialValue > 0) ? pInitialValue : 0.0m;
            TotalOrder = pTotalOrder;

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(_moneyPad, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            //Start Enable or Disable
            _buttonOk.Sensitive = (pInitialValue > 0 && pInitialValue >= pTotalOrder);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, pWindowTitle, windowSize, fixedContent, actionAreaButtons);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events
        //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
        private void _moneyPad_EntryChanged(object sender, EventArgs e)
        {
            Amount = _moneyPad.DeliveryValue;

            if (TotalOrder != 0)
            {
                if (Amount <= TotalOrder)
                    _buttonOk.Sensitive = _moneyPad.Validated;
                else { _buttonOk.Sensitive = false; }
            }
            else
            {
                _buttonOk.Sensitive = _moneyPad.Validated;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Methods
        public static MoneyPadResult RequestDecimalValue(Window pSourceWindow, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
        {
            return RequestDecimalValue(pSourceWindow, string.Empty, pInitialValue, pTotalOrder);
        }

        public static MoneyPadResult RequestDecimalValue(Window pSourceWindow, string pWindowTitle = "", decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
        {
            ResponseType resultResponse;
            decimal resultValue = -1.0m;
            string defaultValue = LogicPOS.Utility.DataConversionUtils.DecimalToString(pInitialValue);

            PosMoneyPadDialog dialog;

            if (pWindowTitle != string.Empty)
            {
                dialog = new PosMoneyPadDialog(pSourceWindow, DialogFlags.DestroyWithParent, pWindowTitle, pInitialValue, pTotalOrder);
            }
            else
            {
                dialog = new PosMoneyPadDialog(pSourceWindow, DialogFlags.DestroyWithParent, pInitialValue, pTotalOrder);
            }

            int response = dialog.Run();
            if (response == (int)ResponseType.Ok)
            {
                resultValue = dialog.Amount;
            }
            resultResponse = (ResponseType)response;
            dialog.Destroy();

            MoneyPadResult result = new MoneyPadResult(resultResponse, resultValue);

            return result;
        }
    }
}
