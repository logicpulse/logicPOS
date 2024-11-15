using Gtk;
using System;
using System.Drawing;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.UI.Extensions;

namespace LogicPOS.UI.Buttons
{
    public class TableButton : TextButton
    {

        private Color _buttonColor;
        private Color _colorPosTablePadTableTableStatusOpenButtonBackground;
        private Color _colorPosTablePadTableTableStatusReservedButtonBackground;
     
        private Label _labelTotalOrStatus;
        private EventBox _eventBoxTotalOrStatus;

        public readonly TableButtonSettings TableSettings;

        public TableButton(TableButtonSettings settings)
            : base(settings,false)
        {
            TableSettings = settings;
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

            switch (TableSettings.TableStatus)
            {
                case TableStatus.Free:
                    SetBackgroundColor(_buttonColor, _eventBoxTotalOrStatus);
                    break;
                case TableStatus.Open:
                    _labelTotalOrStatus.Text = Utility.DataConversionUtils.DecimalToStringCurrency(TableSettings.Total, XPOSettings.ConfigurationSystemCurrency.Acronym);
                    if (TableSettings.OpenedAt != null) labelDateTableOpenOrClosed.Text = string.Format(LocalizedString.Instance["pos_button_label_table_open_at"], TableSettings.OpenedAt.ToString(LogicPOS.Settings.CultureSettings.DateTimeFormatHour));
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

        public void ChangeTableStatus(Guid pTableOid,
                                      TableStatus pTableStatus)
        {
            pos_configurationplacetable xTable = XPOUtility.GetEntityById<pos_configurationplacetable>(pTableOid);
           
            if (pTableStatus == TableStatus.Reserved)
            {
                _labelTotalOrStatus.Text = LocalizedString.Instance["global_reserved_table"];
                _eventBoxTotalOrStatus.VisibleWindow = true;
                SetBackgroundColor(_colorPosTablePadTableTableStatusReservedButtonBackground, _eventBoxTotalOrStatus);
                xTable.TableStatus = TableStatus.Reserved;
                XPOUtility.Audit("TABLE_RESERVED", string.Format(LocalizedString.Instance["audit_message_table_reserved"], xTable.Designation));
            }
            else
            {
                _labelTotalOrStatus.Text = string.Empty;
                _eventBoxTotalOrStatus.VisibleWindow = false;
                SetBackgroundColor(_buttonColor, _eventBoxTotalOrStatus);
                xTable.TableStatus = TableStatus.Free;
                XPOUtility.Audit("TABLE_UNRESERVED", string.Format(LocalizedString.Instance["audit_message_table_unreserved"], xTable.Designation));
            }
 
            TableSettings.TableStatus = pTableStatus;

            xTable.Save();
        }
    }
}
