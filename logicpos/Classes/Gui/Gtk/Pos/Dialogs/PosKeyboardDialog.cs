using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    public class PosKeyboardDialog : PosBaseDialog
    {
        private readonly KeyBoardPad _keyboardPad;
        //Public Properties
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
        public EntryValidation TextEntry
        {
            get { return _keyboardPad.TextEntry; }
        }

        //Constructor
        public PosKeyboardDialog(Window pSourceWindow, DialogFlags pDialogFlags, KeyboardMode pKeyboardMode, string pTextEntry, string pValidationRule) : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_virtual_keyboard");
            Size windowSize = new Size(916, 358);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_keyboard.png";
            string fileKeyboardXML = PathsSettings.Paths["keyboards"] + @"163.xml";

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
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, null);
        }

        //Override Responses - Required to Keep Keyboard in Memory
        protected override void OnResponse(ResponseType pResponse)
        {
            bool useBaseDialogWindowMask = Convert.ToBoolean(GeneralSettings.Settings["useBaseDialogWindowMask"]);

            if (useBaseDialogWindowMask && this.WindowMaskBackground.Visible) this.WindowMaskBackground.Hide();

            this.Hide();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Methods

        public static decimal RequestDecimalValue(Window pSourceWindow, decimal pDefaultValue, bool pUseDefaultValue = true)
        {
            decimal result;
            string regexDecimalGreaterThanZero = LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero;
            string defaultValue = (pUseDefaultValue) ? LogicPOS.Utility.DataConversionUtils.DecimalToString(pDefaultValue) : string.Empty;

            PosKeyboardDialog dialog = new PosKeyboardDialog(pSourceWindow, DialogFlags.DestroyWithParent, KeyboardMode.Numeric, defaultValue, regexDecimalGreaterThanZero);
            int response = dialog.Run();

            if (response == (int)ResponseType.Ok)
            {
                result = decimal.Parse(dialog.Text, CultureSettings.CurrentCultureNumberFormat);
            }
            else
            {
                result = response;
            }
            dialog.Destroy();

            return result;
        }

        public static string RequestAlfaNumericValue(Window pSourceWindow, KeyboardMode pKeyboardMode, string pDefaultValue, bool pUseDefaultValue = true)
        {
            string result;
            string regexAlfaNumeric = LogicPOS.Utility.RegexUtils.RegexAlfaNumeric;
            string defaultValue = (pUseDefaultValue) ? pDefaultValue : string.Empty;

            PosKeyboardDialog dialog = new PosKeyboardDialog(pSourceWindow, DialogFlags.DestroyWithParent, pKeyboardMode, defaultValue, regexAlfaNumeric);
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
