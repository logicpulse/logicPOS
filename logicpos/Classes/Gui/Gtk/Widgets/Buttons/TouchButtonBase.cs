using Gtk;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonBase : Button
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected Fields
        protected EventBox _eventboxBackgroundColor;
        //Public Properties
        private string _token;
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
        private Guid _currentButtonOid;
        public Guid CurrentButtonOid
        {
            get { return _currentButtonOid; }
            set { _currentButtonOid = value; }
        }

        public TouchButtonBase(String pName)
        {
            Name = pName;
            BorderWidth = 1;
            Relief = ReliefStyle.Half;
            CanFocus = false;
            //CanDefault = false;
            //HasDefault = true;
            //HasFocus = true;

            //Clicked += delegate {_log.Debug("TouchButtonBase() ButtonPress: button.Name=" + Name);};
        }

        public TouchButtonBase(String pName, System.Drawing.Color pColor, Widget pWidget, int pWidth, int pHeight)
            : this(pName)
        {
            InitObject(pName, pColor, pWidget, pWidth, pHeight);
        }

        public void InitObject(String pName, System.Drawing.Color pColor, Widget pWidget, int pWidth, int pHeight)
        {
            WidthRequest = pWidth;
            HeightRequest = pHeight;
            _eventboxBackgroundColor = new EventBox();
            SetBackgroundColor(pColor, _eventboxBackgroundColor);
            if (pWidget != null) _eventboxBackgroundColor.Add(pWidget);
            Add(_eventboxBackgroundColor);
            ShowAll();
        }

        public void SetBackgroundColor(System.Drawing.Color pColor, EventBox pTargetEventBox)
        {
            if (pColor == System.Drawing.Color.Transparent)
            {
                pTargetEventBox.VisibleWindow = false;
            }
            else
            {
                Color colNormal = pColor;
                Color colPrelight = Utils.Lighten(colNormal);
                Color colActive = Utils.Lighten(colPrelight);
                Color colInsensitive = Utils.Darken(colNormal);
                Color colSelected = Color.FromArgb(125, 0, 0);
                pTargetEventBox.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colNormal));
                pTargetEventBox.ModifyBg(StateType.Selected, Utils.ColorToGdkColor(colSelected));
                pTargetEventBox.ModifyBg(StateType.Prelight, Utils.ColorToGdkColor(colPrelight));
                pTargetEventBox.ModifyBg(StateType.Active, Utils.ColorToGdkColor(colActive));
                pTargetEventBox.ModifyBg(StateType.Insensitive, Utils.ColorToGdkColor(colInsensitive));
            }
        }
    }
}
