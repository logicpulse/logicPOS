using Gtk;
using logicpos.financial;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.shared;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class MoneyPadResult
    {
        ResponseType _response;
        public ResponseType Response
        {
            get { return _response; }
            set { _response = value; }
        }
        decimal _value;
        public decimal Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public MoneyPadResult(ResponseType pResponse, decimal pValue)
        {
            _response = pResponse;
            _value = pValue;
        }
    }

    class PosMoneyPadDialog : PosBaseDialog
    {
        //UI
        private MoneyPad _moneyPad;
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        private decimal _amount = 0.0m;
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public PosMoneyPadDialog(Window pSourceWindow, DialogFlags pDialogFlags, decimal pInitialValue = 0.0m, decimal pTotalOrder = 0.0m)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle;
            if (pTotalOrder > 0)
            {
                windowTitle = string.Format("{0} - {1} : {2}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_moneypad"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_table_tickets"), FrameworkUtils.DecimalToStringCurrency(pTotalOrder));
            }
            else
            {
                windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_moneypad");
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
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_payments.png");

            //Init MoneyPad
            _moneyPad = new MoneyPad(pSourceWindow, pInitialValue);
            _moneyPad.EntryChanged += _moneyPad_EntryChanged;
            //If pInitialValue defined, Assign it
            _amount = (pInitialValue > 0) ? pInitialValue : 0.0m;

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(_moneyPad, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            //Start Enable or Disable
            _buttonOk.Sensitive = (pInitialValue > 0 && pInitialValue > pTotalOrder) ? true : false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, pWindowTitle, windowSize, fixedContent, actionAreaButtons);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        void _moneyPad_EntryChanged(object sender, EventArgs e)
        {
            _amount = _moneyPad.DeliveryValue;
            _buttonOk.Sensitive = _moneyPad.Validated;
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

            String regexDecimalGreaterThanZero = SettingsApp.RegexDecimalGreaterThanZero;
            String defaultValue = FrameworkUtils.DecimalToString(pInitialValue);

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
