using Gtk;
using logicpos.financial;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.resources.Resources.Localization;
using System;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class EntryBoxValidationDatePickerDialog : EntryBoxValidationButton
    {
        //Private Properties
        private string _windowTitle;
        private string _dateFormat;
        //Public Properties
        private DateTime _dateTime;
        public DateTime Value
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }
        private DateTime _dateTimeMin = DateTime.MinValue;
        public DateTime DateTimeMin
        {
            get { return _dateTimeMin; }
            set { _dateTimeMin = value; }
        }
        private DateTime _dateTimeMax = DateTime.MaxValue;
        public DateTime DateTimeMax
        {
            get { return _dateTimeMax; }
            set { _dateTimeMax = value; }
        }


        //Custom Events
        public event EventHandler ClosePopup;

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), FrameworkUtils.CurrentDateTimeAtomic(), pRule, pRequired)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, pWindowTitle, FrameworkUtils.CurrentDateTimeAtomic(), pRule, pRequired)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, string pRule, bool pRequired, string pDateFormat)
            :this(pSourceWindow, pLabelText, pWindowTitle, FrameworkUtils.CurrentDateTimeAtomic(), pRule, pRequired, pDateFormat)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, DateTime pDateTime, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), pDateTime, pRule, pRequired)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, DateTime pDateTime, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, string.Empty, pDateTime, pRule, pRequired, SettingsApp.DateFormat)
        {
        }
        /* IN005974 -  KeyboardMode.AlfaNumeric makes date fields accept text */
        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, DateTime pDateTime, string pRule, bool pRequired, string pDateFormat)
            :this(pSourceWindow, pLabelText, string.Empty, pDateTime, KeyboardMode.AlfaNumeric, pRule, pRequired, pDateFormat)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, DateTime pDateTime, KeyboardMode pKeyboardMode, string pRule, bool pRequired, string pDateFormat)
            : base(pSourceWindow, pLabelText, pKeyboardMode, pRule, pRequired)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _windowTitle = pWindowTitle;
            _dateTime = pDateTime;
            _dateFormat = pDateFormat;
            //Events
            _button.Clicked += delegate { PopupDialog(); };

            _entryValidation.Changed += delegate
            {

                if (_entryValidation.Validated && _entryValidation.Text != string.Empty)
                {
                    try
                    {
                        _dateTime = Convert.ToDateTime(_entryValidation.Text);
                        //Call Custom Validate for Data Ranges (Min/Max)
                        Validate();
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }
                }
            };
        }

        //Events
        protected void PopupDialog()
        {
            try
            {
                PosDatePickerDialog dialog = null;
                if (_windowTitle == string.Empty)
                {
                    dialog = new PosDatePickerDialog(_sourceWindow, DialogFlags.DestroyWithParent, _dateTime);
                }
                else
                {
                    dialog = new PosDatePickerDialog(_sourceWindow, DialogFlags.DestroyWithParent, _windowTitle, _dateTime);
                }

                ResponseType response = (ResponseType)dialog.Run();
                if (response == ResponseType.Ok)
                {
                    DateTime now = FrameworkUtils.CurrentDateTimeAtomic();
                    //Get Date from Calendar Widget
                    DateTime date = dialog.Calendar.Date;
                    //Transform Date to DateTime, Date + Current Hour
                    _dateTime = new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute, now.Second);
                    //Apply with custom DateFormat, can assign any Format YYYYMMDD, YYYYMMDD HH:MM:SS etc
                    _entryValidation.Text = _dateTime.ToString(_dateFormat);
                    _entryValidation.Validate();
                    //Call Custom Validate for Data Ranges (Min/Max)
                    Validate();

                    //Call Custom Event, Only if OK, if Cancel Dont Trigger Event
                    OnClosePopup();
                }
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Events

        private void OnClosePopup()
        {
            if (ClosePopup != null)
            {
                ClosePopup(this, EventArgs.Empty);
            }
        }

        //Required Custom Validate Over Default EntryValidation for Min and Max Dates
        private void Validate()
        {
            //Revalidate Min/Max Data Range
            if (_entryValidation.Validated && (_dateTime < _dateTimeMin || _dateTime > _dateTimeMax))
            {
                _entryValidation.Validated = false;
            }
            Utils.ValidateUpdateColors(_entryValidation, _entryValidation.Label, _entryValidation.Validated);
        }
    }
}
