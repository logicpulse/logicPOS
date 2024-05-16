using Gtk;
using logicpos.Extensions;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonBase : Button
    {
        //Log4Net
        protected log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected Fields
        protected EventBox _eventboxBackgroundColor;

        public string Token { get; set; }
        private Guid _currentButtonOid;
        public Guid CurrentButtonOid
        {
            get { return _currentButtonOid; }
            set { _currentButtonOid = value; }
        }

        public TouchButtonBase(string pName)
        {
            Name = pName;
            BorderWidth = 1;
            Relief = ReliefStyle.Half;
            CanFocus = false;
            //CanDefault = false;
            //HasDefault = true;
            //HasFocus = true;

            //Clicked += delegate {_logger.Debug("TouchButtonBase() ButtonPress: button.Name=" + Name);};
        }

        public TouchButtonBase(string pName, System.Drawing.Color pColor, Widget pWidget, int pWidth, int pHeight)
            : this(pName)
        {
            InitObject(pName, pColor, pWidget, pWidth, pHeight);
        }

        public void InitObject(string pName, System.Drawing.Color pColor, Widget pWidget, int pWidth, int pHeight)
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
            if (pColor == Color.Transparent)
            {
                pTargetEventBox.VisibleWindow = false;
            }
            else
            {
                Color colNormal = pColor;
                Color colPrelight = colNormal.Lighten();
                Color colActive = colPrelight.Lighten();
                Color colInsensitive = colNormal.Darken();
                Color colSelected = Color.FromArgb(125, 0, 0);

                pTargetEventBox.ModifyBg(StateType.Normal, colNormal.ToGdkColor());
                pTargetEventBox.ModifyBg(StateType.Selected, colSelected.ToGdkColor());
                pTargetEventBox.ModifyBg(StateType.Prelight, colPrelight.ToGdkColor());
                pTargetEventBox.ModifyBg(StateType.Active, colActive.ToGdkColor());
                pTargetEventBox.ModifyBg(StateType.Insensitive, colInsensitive.ToGdkColor());
            }
        }
    }
}
