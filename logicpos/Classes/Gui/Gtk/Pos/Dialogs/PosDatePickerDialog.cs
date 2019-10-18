using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosDatePickerDialog : PosBaseDialog
    {
        //Private Members
        private DateTime _dateTime;
        //UI
        private Fixed _fixedContent;
        //Public Properties
        private Calendar _calendar;
        public Calendar Calendar
        {
            get { return _calendar; }
            set { _calendar = value; }
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : this(pSourceWindow, pDialogFlags, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_datepicker"), FrameworkUtils.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, String pDialogTitle)
            : this(pSourceWindow, pDialogFlags, pDialogTitle, FrameworkUtils.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, DateTime pDateTime)
            : this(pSourceWindow, pDialogFlags, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_datepicker"), pDateTime)
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, String pDialogTitle, DateTime pDateTime)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _dateTime = pDateTime;

            //Init Local Vars
            String windowTitle = pDialogTitle;
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_date_picker.png");
            _windowSize = new Size(600, 373);

            //Init Content
            _fixedContent = new Fixed();

            //Call Init UI
            InitUI();

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, _fixedContent, actionAreaButtons);
        }

        private void InitUI()
        {
            //Init Font Description
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(GlobalFramework.Settings["fontEntryBoxValue"]);
            //Init Calendar
            _calendar = new Calendar();
            _calendar.Date = _dateTime;
            _calendar.ModifyFont(fontDescription);
            _calendar.SetSizeRequest(_windowSize.Width - 13, _windowSize.Height - 120);
              
            _fixedContent.Put(_calendar, 0, 0);
        }
    }
}
