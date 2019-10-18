using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosDatePickerStartEndDateDialog : PosBaseDialog
    {
        private Fixed _fixedContent;
        private EntryBoxValidationDatePickerDialog _entryBoxDateStart;
        private EntryBoxValidationDatePickerDialog _entryBoxDateEnd;

        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;

        private DateTime _dateStart;
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; }
        }
        private DateTime _dateEnd;
        public DateTime DateEnd
        {
            get { return _dateEnd; }
            set { _dateEnd = value; }
        }

        //Overload : Default Dates Start: 1st Day of Month, End Last Day Of Month
        public PosDatePickerStartEndDateDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //pastMonths=0 to Work in Curent Month Range, pastMonths=1 Works in Past Month, pastMonths=2 Two months Ago etc
            int pastMonths = 1;
            DateTime workingDate = FrameworkUtils.CurrentDateTimeAtomic().AddMonths(-pastMonths);
            DateTime firstDayOfMonth = new DateTime(workingDate.Year, workingDate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime dateTimeStart = firstDayOfMonth;
            DateTime dateTimeEnd = lastDayOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59);
    
            InitUI(pDialogFlags, dateTimeStart, dateTimeEnd);
        }
            
        public PosDatePickerStartEndDateDialog(Window pSourceWindow, DialogFlags pDialogFlags, DateTime pDateStart, DateTime pDateEnd)
            : base(pSourceWindow, pDialogFlags)
        {
            //Call Init UI
            InitUI(pDialogFlags, pDateStart, pDateEnd);
        }

        private void InitUI(DialogFlags pDialogFlags, DateTime pDateStart, DateTime pDateEnd)
        {
            //Parameters
            _dateStart = pDateStart;
            _dateEnd = pDateEnd;

            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_datepicket_startend");
            Size windowSize = new Size(300, 255);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_date_picker.png");

            //Init Content
            _fixedContent = new Fixed();

            //Init DateEntry Start
            _entryBoxDateStart = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date_start"), _dateStart, SettingsApp.RegexDate, true);
            _entryBoxDateStart.EntryValidation.Text = _dateStart.ToString(SettingsApp.DateFormat);
            _entryBoxDateStart.EntryValidation.Validate();
            _entryBoxDateStart.ClosePopup += entryBoxDateStart_ClosePopup;    
            //Init DateEntry End
            _entryBoxDateEnd = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date_end"), _dateEnd, SettingsApp.RegexDate, true);
            _entryBoxDateEnd.EntryValidation.Text = _dateEnd.ToString(SettingsApp.DateFormat);
            _entryBoxDateEnd.EntryValidation.Validate();
            _entryBoxDateEnd.ClosePopup += entryBoxDateEnd_ClosePopup;

            VBox vbox = new VBox(true, 0) { WidthRequest = 290 };
            vbox.PackStart(_entryBoxDateStart, true, true, 2);
            vbox.PackStart(_entryBoxDateEnd, true, true, 2);
            
            _fixedContent.Put(vbox, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Start Validated
            Validate();

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _fixedContent, actionAreaButtons);
        }

        private void entryBoxDateStart_ClosePopup(object sender, EventArgs e)
        {
            _dateStart = _entryBoxDateStart.Value;
            Validate();
        }

        private void entryBoxDateEnd_ClosePopup(object sender, EventArgs e)
        {
            _dateEnd = _entryBoxDateEnd.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            Validate();
        }

        private void Validate()
        {
            _buttonOk.Sensitive = (_dateStart < _dateEnd); 
        }
    }
}
