﻿using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using System.Drawing;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    /// <summary>
    /// Class to get input from user, can incremented with other widgets usign VBoxContent
    /// ex in PosConfirmAcronymSeriesDialog Method
    /// </summary>

    internal class PosInputTextDialog : BaseDialog
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

        public PosInputTextDialog(Window parentWindow, DialogFlags pDialogFlags, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
            : this(parentWindow, pDialogFlags, new Size(600, 180), pWindowTitle, pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired)
        {
        }

        public PosInputTextDialog(Window parentWindow, DialogFlags pDialogFlags, Size pSize, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pInitialValue, string pRule, bool pRequired)
            : this(parentWindow, pDialogFlags, pSize, pWindowTitle, pWindowIcon, pEntryLabel, pInitialValue, KeyboardMode.AlfaNumeric, pRule, pRequired)
        {
        }

        public PosInputTextDialog(Window parentWindow, DialogFlags pDialogFlags, Size pSize, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pInitialValue, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
            : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = pWindowTitle;
            Size windowSize = pSize;

            if (!File.Exists(pWindowIcon))
            {
                pWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_system.png";
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
            IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            buttonOk.Sensitive = _entryBoxValidation.EntryValidation.Validated;

            //After Button Construction
            _entryBoxValidation.EntryValidation.Changed += delegate
            {
                _value = _entryBoxValidation.EntryValidation.Text;
                buttonOk.Sensitive = _entryBoxValidation.EntryValidation.Validated;
            };

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.Initialize(this, pDialogFlags, pWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }
    }
}
