using Gtk;
using logicpos.Classes.Enums.Keyboard;
using System;
using System.Collections.Generic;
using LogicPOS.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Utility;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Alerts;
using System.Drawing;
using LogicPOS.UI.Components.InputFields.Validation;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidationDatePickerMultiDates : EventBox
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _debug = false;

        //Private
        private readonly Window _sourceWindow;
        private readonly VBox _vbox;

        public string Title { get; set; }
        public DateTime AllowedPeriodBegin { get; set; }

        public DateTime AllowedPeriodEnd { get; set; }
        internal EntryBoxValidationDatePickerDialog EntryBoxAddDate { get; set; }

        public List<DateTime> Value { get; set; }

        //Events
        public event EventHandler Changed;

        public EntryBoxValidationDatePickerMultiDates(Window parentWindow, string pLabelText)
            : this(parentWindow, pLabelText, string.Empty, null)
        {
        }

        public EntryBoxValidationDatePickerMultiDates(Window parentWindow, string pLabelText, string pWindowTitle)
            : this(parentWindow, pLabelText, pWindowTitle, null)
        {
        }

        public EntryBoxValidationDatePickerMultiDates(Window parentWindow, string pLabelText, List<DateTime> pInitialDatesList)
            : this(parentWindow, pLabelText, string.Empty, pInitialDatesList)
        {
        }

        public EntryBoxValidationDatePickerMultiDates(Window parentWindow, string pLabelText, string pWindowTitle, List<DateTime> pInitialDatesList)
        {
            string initialDate = XPOUtility.CurrentDateTimeAtomic().ToString(CultureSettings.DateFormat);

            //Parameters
            _sourceWindow = parentWindow;

            //Init Dates List
            Value = pInitialDatesList;
            //Init Dates VBox
            _vbox = new VBox(false, 0);

            //Init DateEntry
            EntryBoxAddDate = new EntryBoxValidationDatePickerDialog(parentWindow, pLabelText, pWindowTitle, RegularExpressions.Date, false);
            EntryBoxAddDate.EntryValidation.Text = initialDate;
            EntryBoxAddDate.EntryValidation.Validate();
            EntryBoxAddDate.ClosePopup += _entryBoxAddDate_ClosePopup;

            VBox vboxOuter = new VBox(false, 0);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);
            scrolledWindow.ResizeMode = ResizeMode.Parent;
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(_vbox);
            scrolledWindow.Add(viewport);

            //Initial Values
            if (Value.Count > 0)
            {
                for (int i = 0; i < Value.Count; i++)
                {
                    //Assign current fileName to _entryBoxAddFile, the last added is the Visible One
                    EntryBoxAddDate.EntryValidation.Text = Value[i].ToString(CultureSettings.DateFormat);
                    AddDateTimeEntry(Value[i], false);
                }
            }

            vboxOuter.PackStart(EntryBoxAddDate, false, false, 0);
            vboxOuter.PackStart(scrolledWindow, true, true, 0);
            Add(vboxOuter);
        }

        private void _entryBoxAddDate_ClosePopup(object sender, EventArgs e)
        {
            if (Value.Contains(EntryBoxAddDate.Value))
            {
                CustomAlerts.Error(_sourceWindow)
                             .WithSize(new Size(500, 340))
                             .WithMessage(GeneralUtils.GetResourceByName("dialog_message_datepicker_existing_date_error"))
                             .ShowAlert();
            }
            else
            {
                if (
                    (AllowedPeriodBegin != null && EntryBoxAddDate.Value.Date >= AllowedPeriodBegin.Date) &&
                    (AllowedPeriodEnd != null && EntryBoxAddDate.Value.Date <= AllowedPeriodEnd.Date)
                )
                {
                    AddDateTimeEntry(EntryBoxAddDate.Value, true);
                    OnChange();

                    if (_debug) ListValue();
                }
                else
                {

                    CustomAlerts.Error(_sourceWindow)
                                 .WithSize(new Size(500, 340))
                                 .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_datepicker_existing_date_error_outside_range"),
                                               AllowedPeriodBegin.ToString(CultureSettings.DateFormat),
                                               AllowedPeriodEnd.ToString(CultureSettings.DateFormat))
                                 )
                                 .ShowAlert();
                }
            }
        }

        private void AddDateTimeEntry(DateTime pDateTime, bool pAddDateTimeToList)
        {
            string iconFileName = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, @"Icons/Windows/icon_window_delete_record.png");
            EntryBoxValidationButton entryBoxValidation = new EntryBoxValidationButton(_sourceWindow, string.Format(GeneralUtils.GetResourceByName("global_date"), Value.Count + 1), KeyboardMode.None, RegularExpressions.Date, true, iconFileName);
            entryBoxValidation.EntryValidation.Validate();
            entryBoxValidation.EntryValidation.Sensitive = false;
            //Remove Event
            entryBoxValidation.Button.Clicked += Button_Clicked;
            entryBoxValidation.EntryValidation.Text = pDateTime.ToString(CultureSettings.DateFormat);
            _vbox.PackStart(entryBoxValidation, false, false, 0);
            _vbox.ShowAll();
            if (pAddDateTimeToList) Value.Add(EntryBoxAddDate.Value);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            IconButton button = (IconButton)sender;
            EntryBoxValidationButton entryBoxValidationButton = (button.Parent.Parent.Parent as EntryBoxValidationButton);
            _vbox.Remove(entryBoxValidationButton);
            Value.Remove(Convert.ToDateTime(entryBoxValidationButton.EntryValidation.Text));

            //Trigger Event
            OnChange();

            if (_debug) ListValue();
        }

        private void ListValue()
        {
            for (int i = 0; i < Value.Count; i++)
            {
                _logger.Debug(string.Format("_datesList[{0}/{1}]", Value[i], Value.Count));
            }
            _logger.Debug(string.Format("ToString(): {0}", ToString()));
        }

        override public string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < Value.Count; i++)
            {
                result += Value[i].ToString("yyyy-MM-dd");
                if (i < Value.Count - 1)
                {
                    result += ';';
                }
            }
            return result;
        }

        private void OnChange()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
