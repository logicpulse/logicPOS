using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    /// <summary>
    /// Class to get input from user, can incremented with other widgets usign VBoxContent
    /// ex in PosConfirmAcronymSeriesDialog Method
    /// </summary>

    class PosInputTextDialog : PosBaseDialog
    {
        protected VBox _vbox;
        public VBox VBoxContent
        {
            get { return _vbox; }
            set { _vbox = value; }
        }
        protected EntryBoxValidation _entryBoxValidation;
        public EntryBoxValidation EntryBoxValidation
        {
            get { return _entryBoxValidation; }
            set { _entryBoxValidation = value; }
        }
        protected string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public PosInputTextDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
            : this(pSourceWindow, pDialogFlags, new Size(600, 180), pWindowTitle, pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired)
        {
        }

        public PosInputTextDialog(Window pSourceWindow, DialogFlags pDialogFlags, Size pSize, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pInitialValue, string pRule, bool pRequired)
            :this (pSourceWindow, pDialogFlags, pSize, pWindowTitle, pWindowIcon, pEntryLabel, pInitialValue, KeyboardMode.AlfaNumeric, pRule, pRequired)
        {
        }

        public PosInputTextDialog(Window pSourceWindow, DialogFlags pDialogFlags, Size pSize, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pInitialValue, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = pWindowTitle;
            Size windowSize = pSize;

            if (!File.Exists(pWindowIcon))
            {
                pWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_system.png");
            }

            //Always assign  pInitialValue to Dialog.Value
            _value = pInitialValue;

            //Entry
            _entryBoxValidation = new EntryBoxValidation(this, pEntryLabel, pKeyboardMode, pRule, pRequired);
            if (pInitialValue != string.Empty)
            {
                _entryBoxValidation.EntryValidation.Text = pInitialValue;
            }

            //VBox
            _vbox = new VBox(false, 0) { WidthRequest = windowSize.Width - 12 };
            _vbox.PackStart(_entryBoxValidation, false, false, 0);

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(_vbox, 0, 0);

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
			buttonOk.Sensitive = _entryBoxValidation.EntryValidation.Validated;

            //After Button Construction
            _entryBoxValidation.EntryValidation.Changed += delegate { 
                _value = _entryBoxValidation.EntryValidation.Text;
                buttonOk.Sensitive = _entryBoxValidation.EntryValidation.Validated;
            };

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, pWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }
    }
}
