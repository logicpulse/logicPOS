using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LogicPOS.UI.Components.Modals
{
    public class DateTimePicker : Modal
    {
        public IconButtonWithText BtnOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public Calendar Calendar { get; set; } = new Calendar();

        public DateTimePicker(Window parent) : base(parent,
                                                    GeneralUtils.GetResourceByName("window_title_dialog_datepicker"),
                                                    new Size(600, 373),
                                                    PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_date_picker.png")
        {
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Pango.FontDescription font = Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue);
            Calendar.Date = DateTime.Now;
            Calendar.ModifyFont(font);
            var box = new VBox(false, 0);
            box.PackStart(Calendar, true, true, 0);
            return box;
        }

        public string GetFormattedDateTime()
        {
            var dateTime = Calendar.Date.Date;
            dateTime = dateTime.AddHours(DateTime.Now.Hour);
            dateTime = dateTime.AddMinutes(DateTime.Now.Minute);
            dateTime = dateTime.AddSeconds(DateTime.Now.Second);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
