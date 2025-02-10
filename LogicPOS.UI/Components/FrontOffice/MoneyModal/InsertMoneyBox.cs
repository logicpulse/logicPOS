using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class InsertMoneyBox : Box
    {
        //Settings
        private readonly decimal _decimalMoneyButtonL1Value = AppSettings.Instance.decimalMoneyButtonL1Value;
        private readonly decimal _decimalMoneyButtonL2Value = AppSettings.Instance.decimalMoneyButtonL2Value;
        private readonly decimal _decimalMoneyButtonL3Value = AppSettings.Instance.decimalMoneyButtonL3Value;
        private readonly decimal _decimalMoneyButtonL4Value = AppSettings.Instance.decimalMoneyButtonL4Value;
        private readonly decimal _decimalMoneyButtonL5Value = AppSettings.Instance.decimalMoneyButtonL5Value;
        private readonly decimal _decimalMoneyButtonR1Value = AppSettings.Instance.decimalMoneyButtonR1Value;
        private readonly decimal _decimalMoneyButtonR2Value = AppSettings.Instance.decimalMoneyButtonR2Value;
        private readonly decimal _decimalMoneyButtonR3Value = AppSettings.Instance.decimalMoneyButtonR3Value;
        private readonly decimal _decimalMoneyButtonR4Value = AppSettings.Instance.decimalMoneyButtonR4Value;
        private readonly decimal _decimalMoneyButtonR5Value = AppSettings.Instance.decimalMoneyButtonR5Value;
        //UI
        private readonly NumberPad _numberPad;
        private readonly ValidatableTextBox _entryDeliveryValue;
        private readonly TextButton _buttonKeyMBL1;
        private readonly TextButton _buttonKeyMBL2;
        private readonly TextButton _buttonKeyMBL3;
        private readonly TextButton _buttonKeyMBL4;
        private readonly TextButton _buttonKeyMBL5;
        private readonly TextButton _buttonKeyMBR1;
        private readonly TextButton _buttonKeyMBR2;
        private readonly TextButton _buttonKeyMBR3;
        private readonly TextButton _buttonKeyMBR4;
        private readonly TextButton _buttonKeyMBR5;
        //Helper Vars
        private MoneyPadMode _moneyPadMode = MoneyPadMode.Money;
        private int _tempCursorPosition = 0;

        public decimal DeliveryValue { get; set; } = 0.0m;

        public bool Validated { get; set; }
        //Public Event Handlers
        public event EventHandler EntryChanged;

        public InsertMoneyBox(Window parentWindow, decimal pInitialValue = 0.0m)
        {

            //Settings
            string fontMoneyPadButtonKeys = AppSettings.Instance.fontMoneyPadButtonKeys;
            string fontMoneyPadTextEntry = AppSettings.Instance.fontMoneyPadTextEntry;
            //ButtonLabels
            string moneyButtonL1Label = _decimalMoneyButtonL1Value.ToString("C");
            string moneyButtonL2Label = _decimalMoneyButtonL2Value.ToString("C");
            string moneyButtonL3Label = _decimalMoneyButtonL3Value.ToString("C");
            string moneyButtonL4Label = _decimalMoneyButtonL4Value.ToString("C");
            string moneyButtonL5Label = _decimalMoneyButtonL5Value.ToString("C");
            string moneyButtonR1Label = _decimalMoneyButtonR1Value.ToString("C");
            string moneyButtonR2Label = _decimalMoneyButtonR2Value.ToString("C");
            string moneyButtonR3Label = _decimalMoneyButtonR3Value.ToString("C");
            string moneyButtonR4Label = _decimalMoneyButtonR4Value.ToString("C");
            string moneyButtonR5Label = _decimalMoneyButtonR5Value.ToString("C");

            //Local Vars
            Color colorFont = Color.White;
            Size numberPadButtonSize = new Size(100, 80);
            Size moneyButtonSize = new Size(100, 64);

            //Delivery Entry
            string initialValue = (pInitialValue > 0) ? pInitialValue.ToString() : string.Empty;
            _entryDeliveryValue = new ValidatableTextBox(parentWindow, KeyboardMode.None, RegularExpressions.DecimalNumber, true) { Text = initialValue, Alignment = 0.5F };
            _entryDeliveryValue.ModifyFont(Pango.FontDescription.FromString(fontMoneyPadTextEntry));
            //Dialog Validated Equal to Entry, Its the Only Entry in Dialog
            Validated = _entryDeliveryValue.Validated;
            //Event
            _entryDeliveryValue.Changed += _entry_Changed;

            //NumberPad
            _numberPad = new NumberPad("numberPad", Color.Transparent, fontMoneyPadButtonKeys, (byte)numberPadButtonSize.Width, (byte)numberPadButtonSize.Height);
            _numberPad.Clicked += _numberPad_Clicked;

            //MoneyButtons Left
            _buttonKeyMBL1 = new TextButton(
                new ButtonSettings
                {
                    Name = "touchButtonKeyMBL1_Green",
                    Text = moneyButtonL1Label,
                    Font = fontMoneyPadButtonKeys,
                    FontColor = colorFont,
                    ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height)
                });

            _buttonKeyMBL2 = new TextButton(
              new ButtonSettings
              {
                  Name = "touchButtonKeyMBL2_Green",
                  Text = moneyButtonL2Label,
                  Font = fontMoneyPadButtonKeys,
                  FontColor = colorFont,
                  ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height)
              });

            _buttonKeyMBL3 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBL3_Green", Text = moneyButtonL3Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
            _buttonKeyMBL4 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBL4_Green", Text = moneyButtonL4Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
            _buttonKeyMBL5 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBL5_Green", Text = moneyButtonL5Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
            //MoneyButtons Right
            _buttonKeyMBR1 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBR1_Green", Text = moneyButtonR1Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
            _buttonKeyMBR2 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBR2_Green", Text = moneyButtonR2Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
            _buttonKeyMBR3 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBR3_Green", Text = moneyButtonR3Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
            _buttonKeyMBR4 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBR4_Green", Text = moneyButtonR4Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
            _buttonKeyMBR5 = new TextButton(new ButtonSettings { Name = "touchButtonKeyMBR5_Green", Text = moneyButtonR5Label, Font = fontMoneyPadButtonKeys, FontColor = colorFont, ButtonSize = new Size(moneyButtonSize.Width, moneyButtonSize.Height) });
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

        private void _entry_Changed(object sender, EventArgs e)
        {
            ValidatableTextBox entry = (ValidatableTextBox)sender;
            Validated = entry.Validated;

            if (entry.Validated)
            {
                DeliveryValue = decimal.Parse(_entryDeliveryValue.Text);
            }

            EntryChanged?.Invoke(sender, e);
        }

        private void buttonKeyMB_Clicked(object sender, EventArgs e)
        {
            TextButton button = (TextButton)sender;
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

        private void _numberPad_Clicked(object sender, EventArgs e)
        {
            TextButton button = (TextButton)sender;

            if (_moneyPadMode != MoneyPadMode.NumberPad)
            {
                _entryDeliveryValue.Text = string.Empty;
                _tempCursorPosition = 0;
                _moneyPadMode = MoneyPadMode.NumberPad;
            }

            switch (button.ButtonLabel.Text)
            {
                case "CE":
                    _entryDeliveryValue.DeleteText(_entryDeliveryValue.Position - 1, _entryDeliveryValue.Position);
                    _tempCursorPosition = _entryDeliveryValue.Position;
                    break;
                default:
                    _entryDeliveryValue.InsertText(button.ButtonLabel.Text, ref _tempCursorPosition);
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
                DeliveryValue = 0.0m;
                _moneyPadMode = MoneyPadMode.Money;
            }

            DeliveryValue += pAmount;
            _entryDeliveryValue.Text = DeliveryValue.ToString();
        }
    }
}
