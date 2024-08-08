using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using System;
using System.Drawing;
using LogicPOS.Settings;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

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

    internal class PosMoneyPadDialog : BaseDialog
    {
        //UI
        private MoneyPad _moneyPad;
        private IconButtonWithText _buttonOk;
        private IconButtonWithText _buttonCancel;

        public decimal Amount { get; set; } = 0.0m;

        public decimal TotalOrder { get; set; } = 0.0m;

        public PosMoneyPadDialog(Window parentWindow, DialogFlags pDialogFlags, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
            : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle;
            if (pTotalOrder > 0)
            {
                windowTitle = string.Format("{0} - {1} : {2}", GeneralUtils.GetResourceByName("window_title_dialog_moneypad"), GeneralUtils.GetResourceByName("global_total_table_tickets"), DataConversionUtils.DecimalToStringCurrency(pTotalOrder, XPOSettings.ConfigurationSystemCurrency.Acronym));
            }
            else
            {
                windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_moneypad");
            }

            this.InitObject(parentWindow, pDialogFlags, windowTitle, pInitialValue, pTotalOrder);
        }

        public PosMoneyPadDialog(Window parentWindow, DialogFlags pDialogFlags, string pWindowTitle, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
            : base(parentWindow, pDialogFlags)
        {
            //Init Object
            this.InitObject(parentWindow, pDialogFlags, pWindowTitle, pInitialValue, pTotalOrder);
        }

        public void InitObject(Window parentWindow, DialogFlags pDialogFlags, string pWindowTitle, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
        {
            Size windowSize = new Size(524, 497);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_payments.png";

            //Init MoneyPad
            _moneyPad = new MoneyPad(parentWindow, pInitialValue);
            _moneyPad.EntryChanged += _moneyPad_EntryChanged;
            //If pInitialValue defined, Assign it
            Amount = (pInitialValue > 0) ? pInitialValue : 0.0m;
            TotalOrder = pTotalOrder;

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(_moneyPad, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            //Start Enable or Disable
            _buttonOk.Sensitive = (pInitialValue > 0 && pInitialValue >= pTotalOrder);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, pWindowTitle, windowSize, fixedContent, actionAreaButtons);
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
        public static MoneyPadResult RequestDecimalValue(Window parentWindow, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
        {
            return RequestDecimalValue(parentWindow, string.Empty, pInitialValue, pTotalOrder);
        }

        public static MoneyPadResult RequestDecimalValue(Window parentWindow, string pWindowTitle = "", decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
        {
            ResponseType resultResponse;
            decimal resultValue = -1.0m;
            string defaultValue = DataConversionUtils.DecimalToString(pInitialValue);

            PosMoneyPadDialog dialog;

            if (pWindowTitle != string.Empty)
            {
                dialog = new PosMoneyPadDialog(parentWindow, DialogFlags.DestroyWithParent, pWindowTitle, pInitialValue, pTotalOrder);
            }
            else
            {
                dialog = new PosMoneyPadDialog(parentWindow, DialogFlags.DestroyWithParent, pInitialValue, pTotalOrder);
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
