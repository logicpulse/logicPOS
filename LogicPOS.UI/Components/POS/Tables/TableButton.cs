using Gtk;
using LogicPOS.Api.Enums;
using LogicPOS.Globalization;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class TableButton : TextButton
    {
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
                ButtonSize = AppSettings.Instance.SizePosTableButton,
            }, false)
        {
            Table = table;
            _settings.Widget = CreateComponent();
            Initialize();
        }

        public Widget CreateComponent()
        {
            _colorPosTablePadTableTableStatusOpenButtonBackground = AppSettings.Instance.ColorPosTablePadTableTableStatusOpenButtonBackground;
            _colorPosTablePadTableTableStatusReservedButtonBackground =   AppSettings.Instance.ColorPosTablePadTableTableStatusReservedButtonBackground;

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
                case TableStatus.Open:
                    _labelTotalOrStatus.Text = TablesService.GetTableTotal(Table.Id).ToString("C");
                  
                    if (Table.OpennedAt != null)
                    {
                        labelDateTableOpenOrClosed.Text = string.Format(LocalizedString.Instance["pos_button_label_table_open_at"], Table.OpennedAt.Value.ToString("HH:mm"));
                    }

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
