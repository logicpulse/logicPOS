using Gtk;
using LogicPOS.Api.Enums;
using LogicPOS.Globalization;
using System;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class TableButton : TextButton
    {

        private Color _buttonColor;
        private Color _colorPosTablePadTableTableStatusOpenButtonBackground;
        private Color _colorPosTablePadTableTableStatusReservedButtonBackground;
        private Label _labelTotalOrStatus;
        private EventBox _eventBoxTotalOrStatus;
        public readonly Api.Entities.Table Table;

        public TableButton(Api.Entities.Table table)
            : base(new ButtonSettings
            {
                Name = "buttonTableId",
                Text = table.Designation,
                ButtonSize = LogicPOS.Settings.AppSettings.Instance.sizePosTableButton,
            }, false)
        {
            Table = table;
            _settings.Widget = CreateWidget();
            Initialize();
        }

        public Widget CreateWidget()
        {
            _colorPosTablePadTableTableStatusOpenButtonBackground = LogicPOS.Settings.AppSettings.Instance.colorPosTablePadTableTableStatusOpenButtonBackground;
            _colorPosTablePadTableTableStatusReservedButtonBackground = LogicPOS.Settings.AppSettings.Instance.colorPosTablePadTableTableStatusReservedButtonBackground;

            VBox vbox = new VBox(true, 5) { BorderWidth = 5 };

            SetFont($"Bold {_settings.Font}");


            Label labelDateTableOpenOrClosed = new Label(string.Empty);
            Pango.FontDescription fontDescDateTableOpenOrClosed = Pango.FontDescription.FromString("7");
            labelDateTableOpenOrClosed.ModifyFont(fontDescDateTableOpenOrClosed);

            _labelTotalOrStatus = new Label(string.Empty);
            _eventBoxTotalOrStatus = new EventBox
            {
                _labelTotalOrStatus
            };

            _eventBoxTotalOrStatus.ButtonPressEvent += delegate { Click(); };

            //Pack VBox
            vbox.PackStart(ButtonLabel);
            vbox.PackStart(labelDateTableOpenOrClosed);
            vbox.PackStart(_eventBoxTotalOrStatus);

            switch (Table.Status)
            {
                case TableStatus.Free:
                    SetBackgroundColor(_buttonColor, _eventBoxTotalOrStatus);
                    break;
                case TableStatus.Open:
                    _labelTotalOrStatus.Text = "TotalXD";
                    if (Table.OpennedAt != null) labelDateTableOpenOrClosed.Text = string.Format(LocalizedString.Instance["pos_button_label_table_open_at"], Table.OpennedAt.Value.ToString("HH:mm"));
                    SetBackgroundColor(_colorPosTablePadTableTableStatusOpenButtonBackground, _eventBoxTotalOrStatus);
                    break;
                case TableStatus.Reserved:
                    _labelTotalOrStatus.Text = LocalizedString.Instance["global_reserved_table"];
                    SetBackgroundColor(_colorPosTablePadTableTableStatusReservedButtonBackground, _eventBoxTotalOrStatus);
                    break;
                default:
                    break;
            }

            return vbox;
        }
    }
}
