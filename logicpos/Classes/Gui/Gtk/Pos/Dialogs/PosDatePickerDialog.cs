using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosDatePickerDialog : PosBaseDialog
    {
        //Private Members
        private readonly DateTime _dateTime;
        //UI
        private readonly Fixed _fixedContent;

        public Calendar Calendar { get; set; }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : this(pSourceWindow, pDialogFlags, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_datepicker"), XPOHelper.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pDialogTitle)
            : this(pSourceWindow, pDialogFlags, pDialogTitle, XPOHelper.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, DateTime pDateTime)
            : this(pSourceWindow, pDialogFlags, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_datepicker"), pDateTime)
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pDialogTitle, DateTime pDateTime)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _dateTime = pDateTime;

            //Init Local Vars
            string windowTitle = pDialogTitle;
            string fileDefaultWindowIcon = GeneralSettings.Path["images"] + @"Icons\Windows\icon_window_date_picker.png";
            _windowSize = new Size(600, 373);

            //Init Content
            _fixedContent = new Fixed();

            //Call Init UI
            InitUI();

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, _fixedContent, actionAreaButtons);
        }

        private void InitUI()
        {
            //Init Font Description
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(LogicPOS.Settings.GeneralSettings.Settings["fontEntryBoxValue"]);
            //Init Calendar
            Calendar = new Calendar();
            Calendar.Date = _dateTime;
            Calendar.ModifyFont(fontDescription);
            Calendar.SetSizeRequest(_windowSize.Width - 13, _windowSize.Height - 120);

            _fixedContent.Put(Calendar, 0, 0);
        }
    }
}
