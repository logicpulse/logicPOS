using Gtk;
using System;
using System.Drawing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosDatePickerDialog : BaseDialog
    {
        //Private Members
        private readonly DateTime _dateTime;
        //UI
        private readonly Fixed _fixedContent;

        public Calendar Calendar { get; set; }

        public PosDatePickerDialog(Window parentWindow, DialogFlags pDialogFlags)
            : this(parentWindow, pDialogFlags, LocalizedString.Instance["window_title_dialog_datepicker"], XPOUtility.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window parentWindow, DialogFlags pDialogFlags, string pDialogTitle)
            : this(parentWindow, pDialogFlags, pDialogTitle, XPOUtility.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window parentWindow, DialogFlags pDialogFlags, DateTime pDateTime)
            : this(parentWindow, pDialogFlags, LocalizedString.Instance["window_title_dialog_datepicker"], pDateTime)
        {
        }

        public PosDatePickerDialog(Window parentWindow, DialogFlags pDialogFlags, string pDialogTitle, DateTime pDateTime)
            : base(parentWindow, pDialogFlags)
        {
            //Parameters
            _dateTime = pDateTime;

            //Init Local Vars
            string windowTitle = pDialogTitle;
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_date_picker.png";
            WindowSettings.Size = new Size(600, 373);

            //Init Content
            _fixedContent = new Fixed();

            //Call Init UI
            InitUI();

            //ActionArea Buttons
            IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, WindowSettings.Size, _fixedContent, actionAreaButtons);
        }

        private void InitUI()
        {
            //Init Font Description
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue);
            //Init Calendar
            Calendar = new Calendar();
            Calendar.Date = _dateTime;
            Calendar.ModifyFont(fontDescription);
            Calendar.SetSizeRequest(WindowSettings.Size.Width - 13, WindowSettings.Size.Height - 120);

            _fixedContent.Put(Calendar, 0, 0);
        }
    }
}
