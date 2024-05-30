using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using System;
using logicpos.Classes.Enums.Keyboard;
using LogicPOS.Globalization;
using LogicPOS.UI;
using LogicPOS.Data.XPO.Utility;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidationDatePickerDialog : EntryBoxValidationButton
    {
        //Private Properties
        private readonly string _windowTitle;
        private readonly string _dateFormat;

        public DateTime Value { get; set; }
        public DateTime DateTimeMin { get; set; } = DateTime.MinValue;

        public DateTime DateTimeMax { get; set; } = DateTime.MaxValue;


        //Custom Events
        public event EventHandler ClosePopup;

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_date"), XPOHelper.CurrentDateTimeAtomic(), pRule, pRequired)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, pWindowTitle, XPOHelper.CurrentDateTimeAtomic(), pRule, pRequired)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, string pRule, bool pRequired, string pDateFormat)
            :this(pSourceWindow, pLabelText, pWindowTitle, XPOHelper.CurrentDateTimeAtomic(), pRule, pRequired, pDateFormat)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, DateTime pDateTime, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_date"), pDateTime, pRule, pRequired)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, DateTime pDateTime, string pRule, bool pRequired)
            :this(pSourceWindow, pLabelText, string.Empty, pDateTime, pRule, pRequired, LogicPOS.Settings.CultureSettings.DateFormat)
        {
        }
        /* IN005974 -  KeyboardMode.AlfaNumeric makes date fields accept text */
        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, DateTime pDateTime, string pRule, bool pRequired, string pDateFormat, bool pBOSource = false)
            :this(pSourceWindow, pLabelText, string.Empty, pDateTime, KeyboardMode.AlfaNumeric, pRule, pRequired, pDateFormat, pBOSource)
        {
        }

        public EntryBoxValidationDatePickerDialog(Window pSourceWindow, string pLabelText, string pWindowTitle, DateTime pDateTime, KeyboardMode pKeyboardMode, string pRule, bool pRequired, string pDateFormat, bool pBOSource = false)
            : base(pSourceWindow, pLabelText, pKeyboardMode, pRule, pRequired,"", pBOSource)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _windowTitle = pWindowTitle;
            Value = pDateTime;
            _dateFormat = pDateFormat;
            //Events
            _button.Clicked += delegate { PopupDialog(); };

            _entryValidation.Changed += delegate
            {

                if (_entryValidation.Validated && _entryValidation.Text != string.Empty)
                {
                    try
                    {
                        Value = Convert.ToDateTime(_entryValidation.Text);
                        //Call Custom Validate for Data Ranges (Min/Max)
                        Validate();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message, ex);
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
                    dialog = new PosDatePickerDialog(_sourceWindow, DialogFlags.DestroyWithParent, Value);
                }
                else
                {
                    dialog = new PosDatePickerDialog(_sourceWindow, DialogFlags.DestroyWithParent, _windowTitle, Value);
                }

                ResponseType response = (ResponseType)dialog.Run();
                if (response == ResponseType.Ok)
                {
                    DateTime now = XPOHelper.CurrentDateTimeAtomic();
                    //Get Date from Calendar Widget
                    DateTime date = dialog.Calendar.Date;
                    //Transform Date to DateTime, Date + Current Hour
                    Value = new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute, now.Second);
                    //Apply with custom DateFormat, can assign any Format YYYYMMDD, YYYYMMDD HH:MM:SS etc
                    _entryValidation.Text = Value.ToString(_dateFormat);
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
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Events

        private void OnClosePopup()
        {
            ClosePopup?.Invoke(this, EventArgs.Empty);
        }

        //Required Custom Validate Over Default EntryValidation for Min and Max Dates
        private void Validate()
        {
            //Revalidate Min/Max Data Range
            if (_entryValidation.Validated && (Value < DateTimeMin || Value > DateTimeMax))
            {
                _entryValidation.Validated = false;
            }

            GtkUtils.UpdateWidgetColorsAfterValidation(
                _entryValidation, 
                _entryValidation.Validated, 
                _entryValidation.Label, 
                _entryValidation.Label2, 
                _entryValidation.Label3);
        }
    }
}
