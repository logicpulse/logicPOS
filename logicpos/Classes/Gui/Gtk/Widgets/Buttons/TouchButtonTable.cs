using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.datalayer.Enums;
using System;
using System.Drawing;

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

        private TableStatus _tableStatus;
        public TableStatus TableStatus
        {
            get { return _tableStatus; }
            set { _tableStatus = value; }
        }

        public TouchButtonTable(String pName)
            : base(pName)
        {
        }

        public TouchButtonTable(String pName, Color pColor, String pLabelText, String pFont, int pWidth, int pHeight, TableStatus pTableStatus, decimal pTotal, DateTime pDateOpen, DateTime pDateClosed)
            : base(pName)
        {
            InitObject(pName, pColor, pLabelText, pFont, pTableStatus, pTotal, pDateOpen, pDateClosed);
            base.InitObject(pName, pColor, _widget, pWidth, pHeight);
        }

        public void InitObject(String pName, Color pColor, String pLabelText, String pFont, TableStatus pTableStatus, decimal pTotal, DateTime pDateOpen, DateTime pDateClosed)
        {
            //Init Parameters
            _buttonColor = pColor;
            _tableStatus = pTableStatus;

            //Settings
            _colorPosTablePadTableTableStatusOpenButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosTablePadTableTableStatusOpenButtonBackground"]);
            _colorPosTablePadTableTableStatusReservedButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosTablePadTableTableStatusReservedButtonBackground"]);

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
            _eventBoxTotalOrStatus = new EventBox();
            _eventBoxTotalOrStatus.Add(_labelTotalOrStatus);
            //_eventBoxTotalOrStatus.CanFocus = false;
            //If click in EventBox call button Click Event
            _eventBoxTotalOrStatus.ButtonPressEvent += delegate { Click(); };

            //Pack VBox
            vbox.PackStart(_label);
            vbox.PackStart(labelDateTableOpenOrClosed);
            vbox.PackStart(_eventBoxTotalOrStatus);
            //Pack Final Widget
            _widget = vbox;

            //_log.Debug(string.Format("pLabelText:[{0}], _tableStatus: [{1}]", pLabelText, _tableStatus));
            switch (_tableStatus)
            {
                case TableStatus.Free:
                    SetBackgroundColor(_buttonColor, _eventBoxTotalOrStatus);
                    break;
                case TableStatus.Open:
                    _labelTotalOrStatus.Text = FrameworkUtils.DecimalToStringCurrency(pTotal);
                    if (pDateOpen != null) labelDateTableOpenOrClosed.Text = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_button_label_table_open_at"), pDateOpen.ToString(SettingsApp.DateTimeFormatHour));
                    SetBackgroundColor(_colorPosTablePadTableTableStatusOpenButtonBackground, _eventBoxTotalOrStatus);
                    break;
                case TableStatus.Reserved:
                    _labelTotalOrStatus.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_reserved_table");
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
            pos_configurationplacetable xTable = (pos_configurationplacetable)FrameworkUtils.GetXPGuidObject(typeof(pos_configurationplacetable), pTableOid);
            //_log.Debug(string.Format("1 pTableStatus: [{0}] [{1}]", xTable.Designation, pTableStatus));

            if (pTableStatus == TableStatus.Reserved)
            {
                _labelTotalOrStatus.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_reserved_table");
                _eventBoxTotalOrStatus.VisibleWindow = true;
                SetBackgroundColor(_colorPosTablePadTableTableStatusReservedButtonBackground, _eventBoxTotalOrStatus);
                xTable.TableStatus = TableStatus.Reserved;
                FrameworkUtils.Audit("TABLE_RESERVED", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_table_reserved"), xTable.Designation));
            }
            else
            {
                _labelTotalOrStatus.Text = string.Empty;
                _eventBoxTotalOrStatus.VisibleWindow = false;
                SetBackgroundColor(_buttonColor, _eventBoxTotalOrStatus);
                xTable.TableStatus = TableStatus.Free;
                FrameworkUtils.Audit("TABLE_UNRESERVED", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_table_unreserved"), xTable.Designation));
            }
            //_log.Debug(string.Format("1 pTableStatus: [{0}] [{1}]", xTable.Designation, pTableStatus));
            //Update Status State  
            _tableStatus = pTableStatus;
            //Persist in DB
            xTable.Save();
        }
    }
}
