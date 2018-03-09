using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{

    class MoneyPad : Box
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Settings
        private decimal _decimalMoneyButtonL1Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonL1Value"]);
        private decimal _decimalMoneyButtonL2Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonL2Value"]);
        private decimal _decimalMoneyButtonL3Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonL3Value"]);
        private decimal _decimalMoneyButtonL4Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonL4Value"]);
        private decimal _decimalMoneyButtonL5Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonL5Value"]);
        private decimal _decimalMoneyButtonR1Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonR1Value"]);
        private decimal _decimalMoneyButtonR2Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonR2Value"]);
        private decimal _decimalMoneyButtonR3Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonR3Value"]);
        private decimal _decimalMoneyButtonR4Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonR4Value"]);
        private decimal _decimalMoneyButtonR5Value = FrameworkUtils.StringToDecimal(GlobalFramework.Settings["decimalMoneyButtonR5Value"]);
        //UI
        private NumberPad _numberPad;
        private EntryValidation _entryDeliveryValue;
        private TouchButtonText _buttonKeyMBL1;
        private TouchButtonText _buttonKeyMBL2;
        private TouchButtonText _buttonKeyMBL3;
        private TouchButtonText _buttonKeyMBL4;
        private TouchButtonText _buttonKeyMBL5;
        private TouchButtonText _buttonKeyMBR1;
        private TouchButtonText _buttonKeyMBR2;
        private TouchButtonText _buttonKeyMBR3;
        private TouchButtonText _buttonKeyMBR4;
        private TouchButtonText _buttonKeyMBR5;
        //Helper Vars
        private MoneyPadMode _moneyPadMode = MoneyPadMode.Money;
        private int _tempCursorPosition = 0;
        //Public Members
        private decimal _deliveryValue = 0.0m;
        public decimal DeliveryValue
        {
            get { return _deliveryValue; }
            set { _deliveryValue = value; }
        }
        private bool _validated;
        public bool Validated
        {
            get { return _validated; }
            set { _validated = value; }
        }
        //Public Event Handlers
        public event EventHandler EntryChanged;

        public MoneyPad(Window pSourceWindow, decimal pInitialValue = 0.0m)
        {
            //Settings
            string fontMoneyPadButtonKeys = GlobalFramework.Settings["fontMoneyPadButtonKeys"];
            string fontMoneyPadTextEntry = GlobalFramework.Settings["fontMoneyPadTextEntry"];
            //ButtonLabels
            string moneyButtonL1Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonL1Value);
            string moneyButtonL2Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonL2Value);
            string moneyButtonL3Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonL3Value);
            string moneyButtonL4Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonL4Value);
            string moneyButtonL5Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonL5Value);
            string moneyButtonR1Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonR1Value);
            string moneyButtonR2Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonR2Value);
            string moneyButtonR3Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonR3Value);
            string moneyButtonR4Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonR4Value);
            string moneyButtonR5Label = FrameworkUtils.DecimalToStringCurrency(_decimalMoneyButtonR5Value);

            //Local Vars
            Color colorFont = Color.White;
            Size numberPadButtonSize = new Size(100, 80);
            Size moneyButtonSize = new Size(100, 64);

            //Delivery Entry
            string initialValue = (pInitialValue > 0) ? FrameworkUtils.DecimalToString(pInitialValue) : string.Empty;
            _entryDeliveryValue = new EntryValidation(pSourceWindow, KeyboardMode.None, SettingsApp.RegexDecimal, true) { Text = initialValue, Alignment = 0.5F };
            _entryDeliveryValue.ModifyFont(Pango.FontDescription.FromString(fontMoneyPadTextEntry));
            //Dialog Validated Equal to Entry, Its the Only Entry in Dialog
            _validated = _entryDeliveryValue.Validated;
            //Event
            _entryDeliveryValue.Changed += _entry_Changed;

            //NumberPad
            _numberPad = new NumberPad("numberPad", Color.Transparent, fontMoneyPadButtonKeys, (byte)numberPadButtonSize.Width, (byte)numberPadButtonSize.Height);
            _numberPad.Clicked += _numberPad_Clicked;

            //MoneyButtons Left
            _buttonKeyMBL1 = new TouchButtonText("touchButtonKeyMBL1_Green", Color.Transparent, moneyButtonL1Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBL2 = new TouchButtonText("touchButtonKeyMBL2_Green", Color.Transparent, moneyButtonL2Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBL3 = new TouchButtonText("touchButtonKeyMBL3_Green", Color.Transparent, moneyButtonL3Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBL4 = new TouchButtonText("touchButtonKeyMBL4_Green", Color.Transparent, moneyButtonL4Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBL5 = new TouchButtonText("touchButtonKeyMBL5_Green", Color.Transparent, moneyButtonL5Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            //MoneyButtons Right
            _buttonKeyMBR1 = new TouchButtonText("touchButtonKeyMBR1_Green", Color.Transparent, moneyButtonR1Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBR2 = new TouchButtonText("touchButtonKeyMBR2_Green", Color.Transparent, moneyButtonR2Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBR3 = new TouchButtonText("touchButtonKeyMBR3_Green", Color.Transparent, moneyButtonR3Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBR4 = new TouchButtonText("touchButtonKeyMBR4_Green", Color.Transparent, moneyButtonR4Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            _buttonKeyMBR5 = new TouchButtonText("touchButtonKeyMBR5_Green", Color.Transparent, moneyButtonR5Label, fontMoneyPadButtonKeys, colorFont, (byte)moneyButtonSize.Width, (byte)moneyButtonSize.Height);
            //Events
            _buttonKeyMBL1.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBL2.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBL3.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBL4.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBL5.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBR1.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBR2.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBR3.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBR4.Clicked += buttonKeyMB_Clicked;
            _buttonKeyMBR5.Clicked += buttonKeyMB_Clicked;

            VBox vboxMoneyButtonsLeft = new VBox(true, 0);
            vboxMoneyButtonsLeft.PackStart(_buttonKeyMBL1, true, true, 0);
            vboxMoneyButtonsLeft.PackStart(_buttonKeyMBL2, true, true, 0);
            vboxMoneyButtonsLeft.PackStart(_buttonKeyMBL3, true, true, 0);
            vboxMoneyButtonsLeft.PackStart(_buttonKeyMBL4, true, true, 0);
            vboxMoneyButtonsLeft.PackStart(_buttonKeyMBL5, true, true, 0);

            VBox vboxMoneyButtonsRight = new VBox(true, 0);
            vboxMoneyButtonsRight.PackStart(_buttonKeyMBR1, true, true, 0);
            vboxMoneyButtonsRight.PackStart(_buttonKeyMBR2, true, true, 0);
            vboxMoneyButtonsRight.PackStart(_buttonKeyMBR3, true, true, 0);
            vboxMoneyButtonsRight.PackStart(_buttonKeyMBR4, true, true, 0);
            vboxMoneyButtonsRight.PackStart(_buttonKeyMBR5, true, true, 0);

            HBox hboxInput = new HBox(false, 5);
            hboxInput.PackStart(vboxMoneyButtonsLeft);
            hboxInput.PackStart(_numberPad);
            hboxInput.PackStart(vboxMoneyButtonsRight);

            //Vbox
            VBox vboxNumberPad = new VBox(false, 0);
            vboxNumberPad.PackStart(_entryDeliveryValue, false, true, 5);
            vboxNumberPad.PackStart(hboxInput, true, true, 5);

            //Pack It
            this.Add(vboxNumberPad);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Event Handlers

        void _entry_Changed(object sender, EventArgs e)
        {
            EntryValidation entry = (EntryValidation)sender;
            _validated = entry.Validated;
            _deliveryValue = FrameworkUtils.StringToDecimal(_entryDeliveryValue.Text);
            if (EntryChanged != null) EntryChanged(sender, e);
        }

        void buttonKeyMB_Clicked(object sender, EventArgs e)
        {
            TouchButtonText button = (TouchButtonText)sender;
            decimal value = 0.0m;

            switch (button.Name)
            {
                //Left
                case "touchButtonKeyMBL1_Green":
                    value = _decimalMoneyButtonL1Value;
                    break;
                case "touchButtonKeyMBL2_Green":
                    value = _decimalMoneyButtonL2Value;
                    break;
                case "touchButtonKeyMBL3_Green":
                    value = _decimalMoneyButtonL3Value;
                    break;
                case "touchButtonKeyMBL4_Green":
                    value = _decimalMoneyButtonL4Value;
                    break;
                case "touchButtonKeyMBL5_Green":
                    value = _decimalMoneyButtonL5Value;
                    break;
                //Right
                case "touchButtonKeyMBR1_Green":
                    value = _decimalMoneyButtonR1Value;
                    break;
                case "touchButtonKeyMBR2_Green":
                    value = _decimalMoneyButtonR2Value;
                    break;
                case "touchButtonKeyMBR3_Green":
                    value = _decimalMoneyButtonR3Value;
                    break;
                case "touchButtonKeyMBR4_Green":
                    value = _decimalMoneyButtonR4Value;
                    break;
                case "touchButtonKeyMBR5_Green":
                    value = _decimalMoneyButtonR5Value;
                    break;
            }
            AddMoney(value);
        }

        void _numberPad_Clicked(object sender, EventArgs e)
        {
            TouchButtonText button = (TouchButtonText)sender;

            if (_moneyPadMode != MoneyPadMode.NumberPad)
            {
                _entryDeliveryValue.Text = string.Empty;
                _tempCursorPosition = 0;
                _moneyPadMode = MoneyPadMode.NumberPad;
            }

            switch (button.LabelText)
            {
                case "CE":
                    _entryDeliveryValue.DeleteText(_entryDeliveryValue.Position - 1, _entryDeliveryValue.Position);
                    _tempCursorPosition = _entryDeliveryValue.Position;
                    break;
                default:
                    _entryDeliveryValue.InsertText(button.LabelText, ref _tempCursorPosition);
                    _entryDeliveryValue.Position = _tempCursorPosition;
                    break;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Functions
        private void AddMoney(decimal pAmount)
        {
            if (_moneyPadMode != MoneyPadMode.Money)
            {
                _deliveryValue = 0.0m;
                _moneyPadMode = MoneyPadMode.Money;
            }

            _deliveryValue += pAmount;
            _entryDeliveryValue.Text = FrameworkUtils.DecimalToString(_deliveryValue);
        }
    }
}
