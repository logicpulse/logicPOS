using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    public class PosKeyboardDialog : PosBaseDialog
    {
        KeyBoardPad _keyboardPad;
        //Public Properties
        public String Text
        {
            get { return _keyboardPad.TextEntry.Text; }
            set { _keyboardPad.TextEntry.Text = value; }
        }
        public String Rule
        {
            get { return _keyboardPad.TextEntry.Rule; }
            set { _keyboardPad.TextEntry.Rule = value; }
        }
        public EntryValidation TextEntry
        {
            get { return _keyboardPad.TextEntry; }
        }

        //Constructor
        public PosKeyboardDialog(Window pSourceWindow, DialogFlags pDialogFlags, KeyboardMode pKeyboardMode, String pTextEntry, String pValidationRule): base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_virtual_keyboard");
            Size windowSize = new Size(916, 358);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_keyboard.png");
            String fileKeyboardXML = FrameworkUtils.OSSlash(GlobalFramework.Path["keyboards"] + @"163.xml");

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
            Boolean useBaseDialogWindowMask = Convert.ToBoolean(GlobalFramework.Settings["useBaseDialogWindowMask"]);

            if (useBaseDialogWindowMask && this.WindowMaskBackground.Visible) this.WindowMaskBackground.Hide();

            this.Hide();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Methods

        public static decimal RequestDecimalValue(Window pSourceWindow, decimal pDefaultValue, bool pUseDefaultValue = true)
        {
            decimal result;
            String regexDecimalGreaterThanZero = SettingsApp.RegexDecimalGreaterThanZero;
            String defaultValue = (pUseDefaultValue) ? FrameworkUtils.DecimalToString(pDefaultValue) : string.Empty;

            PosKeyboardDialog dialog = new PosKeyboardDialog(pSourceWindow, DialogFlags.DestroyWithParent, KeyboardMode.Numeric, defaultValue, regexDecimalGreaterThanZero);
            int response = dialog.Run();

            if (response == (int)ResponseType.Ok)
            {
                result = decimal.Parse(dialog.Text, GlobalFramework.CurrentCultureNumberFormat);
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
            String regexAlfaNumeric = SettingsApp.RegexAlfaNumeric;
            String defaultValue = (pUseDefaultValue) ? pDefaultValue : string.Empty;

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
