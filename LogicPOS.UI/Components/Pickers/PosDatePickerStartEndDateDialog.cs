﻿using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using System;
using System.Drawing;
using LogicPOS.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosDatePickerStartEndDateDialog : BaseDialog
    {
        private Fixed _fixedContent;
        private EntryBoxValidationDatePickerDialog _entryBoxDateStart;
        private EntryBoxValidationDatePickerDialog _entryBoxDateEnd;

        private IconButtonWithText _buttonOk;
        private IconButtonWithText _buttonCancel;

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        //Overload : Default Dates Start: 1st Day of Month, End Last Day Of Month
        public PosDatePickerStartEndDateDialog(Window parentWindow, DialogFlags pDialogFlags)
            : base(parentWindow, pDialogFlags)
        {
            //pastMonths=0 to Work in Curent Month Range, pastMonths=1 Works in Past Month, pastMonths=2 Two months Ago etc
            int pastMonths = 1;
            DateTime workingDate = XPOUtility.CurrentDateTimeAtomic().AddMonths(-pastMonths);
            DateTime firstDayOfMonth = new DateTime(workingDate.Year, workingDate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime dateTimeStart = firstDayOfMonth;
            DateTime dateTimeEnd = lastDayOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59);

            InitUI(pDialogFlags, dateTimeStart, dateTimeEnd);
        }

        public PosDatePickerStartEndDateDialog(Window parentWindow, DialogFlags pDialogFlags, DateTime pDateStart, DateTime pDateEnd)
            : base(parentWindow, pDialogFlags)
        {
            //Call Init UI
            InitUI(pDialogFlags, pDateStart, pDateEnd);
        }

        private void InitUI(DialogFlags pDialogFlags, DateTime pDateStart, DateTime pDateEnd)
        {
            //Parameters
            DateStart = pDateStart;
            DateEnd = pDateEnd;

            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_datepicket_startend");
            Size windowSize = new Size(300, 255);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_date_picker.png";

            //Init Content
            _fixedContent = new Fixed();

            //Init DateEntry Start
            _entryBoxDateStart = new EntryBoxValidationDatePickerDialog(this, GeneralUtils.GetResourceByName("global_date_start"), DateStart, RegexUtils.RegexDate, true);
            _entryBoxDateStart.EntryValidation.Text = DateStart.ToString(CultureSettings.DateFormat);
            _entryBoxDateStart.EntryValidation.Validate();
            _entryBoxDateStart.ClosePopup += entryBoxDateStart_ClosePopup;
            _entryBoxDateStart.EntryValidation.Changed += entryBoxDateStartEntryValidation_Changed;
            //Init DateEntry End
            _entryBoxDateEnd = new EntryBoxValidationDatePickerDialog(this, GeneralUtils.GetResourceByName("global_date_end"), DateEnd, RegexUtils.RegexDate, true);
            _entryBoxDateEnd.EntryValidation.Text = DateEnd.ToString(CultureSettings.DateFormat);
            _entryBoxDateEnd.EntryValidation.Validate();
            _entryBoxDateEnd.ClosePopup += entryBoxDateEnd_ClosePopup;
            _entryBoxDateEnd.EntryValidation.Changed += entryBoxDateEndEntryValidation_Changed;

            VBox vbox = new VBox(true, 0) { WidthRequest = 290 };
            vbox.PackStart(_entryBoxDateStart, true, true, 2);
            vbox.PackStart(_entryBoxDateEnd, true, true, 2);

            _fixedContent.Put(vbox, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Start Validated
            Validate();

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _fixedContent, actionAreaButtons);
        }

        private void entryBoxDateEndEntryValidation_Changed(object sender, EventArgs e)
        {
            try
            {
                DateEnd = Convert.ToDateTime(_entryBoxDateEnd.EntryValidation.Text).AddHours(23).AddMinutes(59).AddSeconds(59);
                Validate();
            }
            catch { _buttonOk.Sensitive = false; }
        }

        private void entryBoxDateStartEntryValidation_Changed(object sender, EventArgs e)
        {
            try
            {
                DateStart = Convert.ToDateTime(_entryBoxDateStart.EntryValidation.Text);
                Validate();
            }
            catch { _buttonOk.Sensitive = false; }

        }

        private void entryBoxDateStart_ClosePopup(object sender, EventArgs e)
        {
            DateStart = _entryBoxDateStart.Value;
            Validate();

        }

        private void entryBoxDateEnd_ClosePopup(object sender, EventArgs e)
        {
            DateEnd = _entryBoxDateEnd.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            Validate();
        }

        private void Validate()
        {
            _buttonOk.Sensitive = (DateStart < DateEnd);
        }
    }
}
