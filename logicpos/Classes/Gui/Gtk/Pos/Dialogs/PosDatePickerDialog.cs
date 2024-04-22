using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.shared.App;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosDatePickerDialog : PosBaseDialog
    {
        //Private Members
        private readonly DateTime _dateTime;
        //UI
        private readonly Fixed _fixedContent;
        //Public Properties
        private Calendar _calendar;
        public Calendar Calendar
        {
            get { return _calendar; }
            set { _calendar = value; }
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : this(pSourceWindow, pDialogFlags, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_datepicker"), DataLayerUtils.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pDialogTitle)
            : this(pSourceWindow, pDialogFlags, pDialogTitle, DataLayerUtils.CurrentDateTimeAtomic())
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, DateTime pDateTime)
            : this(pSourceWindow, pDialogFlags, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_datepicker"), pDateTime)
        {
        }

        public PosDatePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pDialogTitle, DateTime pDateTime)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _dateTime = pDateTime;

            //Init Local Vars
            string windowTitle = pDialogTitle;
            string fileDefaultWindowIcon = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_date_picker.png");
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
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(DataLayerFramework.Settings["fontEntryBoxValue"]);
            //Init Calendar
            _calendar = new Calendar();
            _calendar.Date = _dateTime;
            _calendar.ModifyFont(fontDescription);
            _calendar.SetSizeRequest(_windowSize.Width - 13, _windowSize.Height - 120);

            _fixedContent.Put(_calendar, 0, 0);
        }
    }
}
