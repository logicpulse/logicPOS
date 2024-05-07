using Gtk;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.Extensions;
using logicpos.shared.App;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonTable : TouchButtonText
    {
        //Colors
        private Color _buttonColor;
        private Color _colorPosTablePadTableTableStatusOpenButtonBackground;
        private Color _colorPosTablePadTableTableStatusReservedButtonBackground;
        //Ui
        private Label _labelTotalOrStatus;
        private EventBox _eventBoxTotalOrStatus;

        public TableStatus TableStatus { get; set; }

        public TouchButtonTable(string pName)
            : base(pName)
        {
        }

        public TouchButtonTable(string pName, Color pColor, string pLabelText, string pFont, int pWidth, int pHeight, TableStatus pTableStatus, decimal pTotal, DateTime pDateOpen, DateTime pDateClosed)
            : base(pName)
        {
            InitObject(pName, pColor, pLabelText, pFont, pTableStatus, pTotal, pDateOpen, pDateClosed);
            InitObject(pName, pColor, _widget, pWidth, pHeight);
        }

        public void InitObject(string pName, Color pColor, string pLabelText, string pFont, TableStatus pTableStatus, decimal pTotal, DateTime pDateOpen, DateTime pDateClosed)
        {
            //Init Parameters
            _buttonColor = pColor;
            TableStatus = pTableStatus;

            //Settings
            _colorPosTablePadTableTableStatusOpenButtonBackground = LogicPOS.Settings.GeneralSettings.Settings["colorPosTablePadTableTableStatusOpenButtonBackground"].StringToColor();
            _colorPosTablePadTableTableStatusReservedButtonBackground = LogicPOS.Settings.GeneralSettings.Settings["colorPosTablePadTableTableStatusReservedButtonBackground"].StringToColor();

            //Initialize UI Components
            VBox vbox = new VBox(true, 5) { BorderWidth = 5 };
            //Button base Label
            _label = new Label(pLabelText);
            SetFont(string.Format("Bold {0}", pFont));
            //Label for Date
            Label labelDateTableOpenOrClosed = new Label(string.Empty);
            Pango.FontDescription fontDescDateTableOpenOrClosed = Pango.FontDescription.FromString("7");
            labelDateTableOpenOrClosed.ModifyFont(fontDescDateTableOpenOrClosed);
            //Label for Total or Status 
            _labelTotalOrStatus = new Label(string.Empty);
            _eventBoxTotalOrStatus = new EventBox
            {
                _labelTotalOrStatus
            };
            //_eventBoxTotalOrStatus.CanFocus = false;
            //If click in EventBox call button Click Event
            _eventBoxTotalOrStatus.ButtonPressEvent += delegate { Click(); };

            //Pack VBox
            vbox.PackStart(_label);
            vbox.PackStart(labelDateTableOpenOrClosed);
            vbox.PackStart(_eventBoxTotalOrStatus);
            //Pack Final Widget
            _widget = vbox;

            //_logger.Debug(string.Format("pLabelText:[{0}], _tableStatus: [{1}]", pLabelText, _tableStatus));
            switch (TableStatus)
            {
                case TableStatus.Free:
                    SetBackgroundColor(_buttonColor, _eventBoxTotalOrStatus);
                    break;
                case TableStatus.Open:
                    _labelTotalOrStatus.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(pTotal, SharedSettings.ConfigurationSystemCurrency.Acronym);
                    if (pDateOpen != null) labelDateTableOpenOrClosed.Text = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "pos_button_label_table_open_at"), pDateOpen.ToString(LogicPOS.Settings.CultureSettings.DateTimeFormatHour));
                    SetBackgroundColor(_colorPosTablePadTableTableStatusOpenButtonBackground, _eventBoxTotalOrStatus);
                    break;
                case TableStatus.Reserved:
                    _labelTotalOrStatus.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_reserved_table");
                    SetBackgroundColor(_colorPosTablePadTableTableStatusReservedButtonBackground, _eventBoxTotalOrStatus);
                    break;
                default:
                    break;
            }
        }

        //Change and Persist TableStatus
        public void ChangeTableStatus(Guid pTableOid, TableStatus pTableStatus)
        {
            //Get Target Table
            pos_configurationplacetable xTable = (pos_configurationplacetable)XPOHelper.GetXPGuidObject(typeof(pos_configurationplacetable), pTableOid);
            //_logger.Debug(string.Format("1 pTableStatus: [{0}] [{1}]", xTable.Designation, pTableStatus));

            if (pTableStatus == TableStatus.Reserved)
            {
                _labelTotalOrStatus.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_reserved_table");
                _eventBoxTotalOrStatus.VisibleWindow = true;
                SetBackgroundColor(_colorPosTablePadTableTableStatusReservedButtonBackground, _eventBoxTotalOrStatus);
                xTable.TableStatus = TableStatus.Reserved;
                SharedUtils.Audit("TABLE_RESERVED", string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "audit_message_table_reserved"), xTable.Designation));
            }
            else
            {
                _labelTotalOrStatus.Text = string.Empty;
                _eventBoxTotalOrStatus.VisibleWindow = false;
                SetBackgroundColor(_buttonColor, _eventBoxTotalOrStatus);
                xTable.TableStatus = TableStatus.Free;
                SharedUtils.Audit("TABLE_UNRESERVED", string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "audit_message_table_unreserved"), xTable.Designation));
            }
            //_logger.Debug(string.Format("1 pTableStatus: [{0}] [{1}]", xTable.Designation, pTableStatus));
            //Update Status State  
            TableStatus = pTableStatus;
            //Persist in DB
            xTable.Save();
        }
    }
}
