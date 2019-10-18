using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class EntryBoxValidationDatePickerMultiDates : EventBox
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _debug = false;

        //Private
        private Window _sourceWindow;
        private VBox _vbox;
        //Public
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private DateTime _allowedPeriodBegin;
        public DateTime AllowedPeriodBegin
        {
            get { return _allowedPeriodBegin; }
            set { _allowedPeriodBegin = value; }
        }
        private DateTime _allowedPeriodEnd;
        public DateTime AllowedPeriodEnd
        {
            get { return _allowedPeriodEnd; }
            set { _allowedPeriodEnd = value; }
        }
        private EntryBoxValidationDatePickerDialog _entryBoxAddDate;
        internal EntryBoxValidationDatePickerDialog EntryBoxAddDate
        {
            get { return _entryBoxAddDate; }
            set { _entryBoxAddDate = value; }
        }
        private List<DateTime> _datesList;
        public List<DateTime> Value
        {
            get { return _datesList; }
            set { _datesList = value; }
        }

        //Events
        public event EventHandler Changed;

        public EntryBoxValidationDatePickerMultiDates(Window pSourceWindow, string pLabelText)
            : this(pSourceWindow, pLabelText, string.Empty, null)
        {
        }

        public EntryBoxValidationDatePickerMultiDates(Window pSourceWindow, string pLabelText, string pWindowTitle)
            : this(pSourceWindow, pLabelText, pWindowTitle, null)
        {
        }

        public EntryBoxValidationDatePickerMultiDates(Window pSourceWindow, string pLabelText, List<DateTime> pInitialDatesList)
            : this(pSourceWindow, pLabelText, string.Empty, pInitialDatesList)
        {
        }

        public EntryBoxValidationDatePickerMultiDates(Window pSourceWindow, string pLabelText, string pWindowTitle, List<DateTime> pInitialDatesList)
        {
            string initialDate = FrameworkUtils.CurrentDateTimeAtomic().ToString(SettingsApp.DateFormat);

            //Parameters
            _sourceWindow = pSourceWindow;

            //Init Dates List
            _datesList = pInitialDatesList;
            //Init Dates VBox
            _vbox = new VBox(false, 0);

            //Init DateEntry
            _entryBoxAddDate = new EntryBoxValidationDatePickerDialog(pSourceWindow, pLabelText, pWindowTitle, SettingsApp.RegexDate, false);
            _entryBoxAddDate.EntryValidation.Text = initialDate;
            _entryBoxAddDate.EntryValidation.Validate();
            _entryBoxAddDate.ClosePopup += _entryBoxAddDate_ClosePopup;

            VBox vboxOuter = new VBox(false, 0);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);
            scrolledWindow.ResizeMode = ResizeMode.Parent;
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(_vbox);
            scrolledWindow.Add(viewport);

            //Initial Values
            if (_datesList.Count > 0)
            {
                for (int i = 0; i < _datesList.Count; i++)
                {
                    //Assign current fileName to _entryBoxAddFile, the last added is the Visible One
                    _entryBoxAddDate.EntryValidation.Text = _datesList[i].ToString(SettingsApp.DateFormat);
                    AddDateTimeEntry(_datesList[i], false);
                }
            }

            vboxOuter.PackStart(_entryBoxAddDate, false, false, 0);
            vboxOuter.PackStart(scrolledWindow, true, true, 0);
            Add(vboxOuter);
        }

        private void _entryBoxAddDate_ClosePopup(object sender, EventArgs e)
        {
            if (_datesList.Contains(_entryBoxAddDate.Value))
            {
                Utils.ShowMessageTouch(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_datepicker_existing_date_error"));
            }
            else
            {
                //Check Valid Date Range
                if (
                    (_allowedPeriodBegin != null && _entryBoxAddDate.Value.Date >= _allowedPeriodBegin.Date) &&
                    (_allowedPeriodEnd != null && _entryBoxAddDate.Value.Date <= _allowedPeriodEnd.Date)
                )
                {
                    //Add Date
                    AddDateTimeEntry(_entryBoxAddDate.Value, true);

                    //Trigger Event
                    OnChange();

                    if (_debug) ListValue();
                }
                //Failled: Invalid Date Range
                else
                {
                    Utils.ShowMessageTouch(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), 
                        string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_datepicker_existing_date_error_outside_range"), 
                            _allowedPeriodBegin.ToString(SettingsApp.DateFormat),
                            _allowedPeriodEnd.ToString(SettingsApp.DateFormat)
                        )
                    );
                }
            }
        }

        private void AddDateTimeEntry(DateTime pDateTime, bool pAddDateTimeToList)
        {
            string iconFileName = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], @"Icons/Windows/icon_window_delete_record.png"));
            EntryBoxValidationButton entryBoxValidation = new EntryBoxValidationButton(_sourceWindow, string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), _datesList.Count + 1), KeyboardMode.None, SettingsApp.RegexDate, true, iconFileName);
            entryBoxValidation.EntryValidation.Validate();
            entryBoxValidation.EntryValidation.Sensitive = false;
            //Remove Event
            entryBoxValidation.Button.Clicked += Button_Clicked;
            entryBoxValidation.EntryValidation.Text = pDateTime.ToString(SettingsApp.DateFormat);
            _vbox.PackStart(entryBoxValidation, false, false, 0);
            _vbox.ShowAll();
            if (pAddDateTimeToList) _datesList.Add(_entryBoxAddDate.Value);
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            TouchButtonIcon button = (TouchButtonIcon)sender;
            EntryBoxValidationButton entryBoxValidationButton = (button.Parent.Parent.Parent as EntryBoxValidationButton);
            _vbox.Remove(entryBoxValidationButton);
            _datesList.Remove(Convert.ToDateTime(entryBoxValidationButton.EntryValidation.Text));

            //Trigger Event
            OnChange();

            if (_debug) ListValue();
        }

        private void ListValue()
        {
            for (int i = 0; i < _datesList.Count; i++)
            {
                _log.Debug(string.Format("_datesList[{0}/{1}]", _datesList[i], _datesList.Count));
            }
            _log.Debug(string.Format("ToString(): {0}", ToString()));
        }

        override public string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < _datesList.Count; i++)
            {
                result += _datesList[i].ToString("yyyy-MM-dd");
                if (i < _datesList.Count - 1)
                {
                    result += ';';
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void OnChange()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }
    }
}
