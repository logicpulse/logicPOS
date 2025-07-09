using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.Globalization;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    public class PosKeyboardDialog : BaseDialog
    {
        private readonly KeyBoardPad _keyboardPad;

        public string Text
        {
            get { return _keyboardPad.TextEntry.Text; }
            set { _keyboardPad.TextEntry.Text = value; }
        }
        public string Rule
        {
            get { return _keyboardPad.TextEntry.Rule; }
            set { _keyboardPad.TextEntry.Rule = value; }
        }
        public ValidatableTextBox TextEntry
        {
            get { return _keyboardPad.TextEntry; }
        }

        //Constructor
        public PosKeyboardDialog(
            Window parentWindow,
            DialogFlags pDialogFlags,
            KeyboardMode pKeyboardMode,
            string pTextEntry,
            string pValidationRule) : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_virtual_keyboard");
            Size windowSize = new Size(916, 358);
            string fileDefaultWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_keyboard.png";
            string fileKeyboardXML = System.IO.Path.Combine(AppSettings.Paths.Keyboards, @"163.xml");

            //Init Content
            Fixed fixedContent = new Fixed();

            //Initialize Virtual Keyboard
            _keyboardPad = new KeyBoardPad(fileKeyboardXML);

            //Pack It
            fixedContent.Put(_keyboardPad, 0, 10);

            //Assign dialog to keyboard ParentDialog Public Properties
            _keyboardPad.ParentDialog = this;
            _keyboardPad.KeyboardMode = pKeyboardMode;

            //Assign Parameters
            if (pTextEntry != string.Empty) _keyboardPad.TextEntry.Text = pTextEntry;
            if (pValidationRule != string.Empty) _keyboardPad.TextEntry.Rule = pValidationRule;
            _keyboardPad.TextEntry.Validate();

            if (pKeyboardMode == KeyboardMode.AlfaPassword)
            {
                _keyboardPad.TextEntry.InvisibleChar = '*';
                _keyboardPad.TextEntry.Visibility = false;
            }

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, null);
        }

        //Override Responses - Required to Keep Keyboard in Memory
        protected override void OnResponse(ResponseType pResponse)
        {
            bool useBaseDialogWindowMask = Convert.ToBoolean(AppSettings.Instance.UseBaseDialogWindowMask);

            if (useBaseDialogWindowMask && this.WindowSettings.Mask.Visible) this.WindowSettings.Mask.Hide();

            this.Hide();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Methods

        public static decimal RequestDecimalValue(Window parentWindow, decimal pDefaultValue, bool pUseDefaultValue = true)
        {
            decimal result;
            string regexDecimalGreaterThanZero = RegularExpressions.DecimalGreaterThanZero;
            string defaultValue = (pUseDefaultValue) ? pDefaultValue.ToString() : string.Empty;

            PosKeyboardDialog dialog = new PosKeyboardDialog(parentWindow, DialogFlags.DestroyWithParent, KeyboardMode.Numeric, defaultValue, regexDecimalGreaterThanZero);
            int response = dialog.Run();

            if (response == (int)ResponseType.Ok)
            {
                result = decimal.Parse(dialog.Text, CultureInfo.CurrentCulture);
            }
            else
            {
                result = response;
            }
            dialog.Destroy();

            return result;
        }

        public static string RequestAlfaNumericValue(Window parentWindow, KeyboardMode pKeyboardMode, string pDefaultValue, bool pUseDefaultValue = true)
        {
            string result;
            string regexAlfaNumeric = RegularExpressions.AlfaNumeric;
            string defaultValue = (pUseDefaultValue) ? pDefaultValue : string.Empty;

            PosKeyboardDialog dialog = new PosKeyboardDialog(parentWindow, DialogFlags.DestroyWithParent, pKeyboardMode, defaultValue, regexAlfaNumeric);
            int response = dialog.Run();

            if (response == (int)ResponseType.Ok)
            {
                result = dialog.Text;
            }
            else
            {
                result = string.Empty;
            }
            dialog.Destroy();

            return result;
        }
    }
}
