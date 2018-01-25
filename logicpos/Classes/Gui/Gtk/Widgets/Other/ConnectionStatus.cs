using Gtk;
using logicpos.App;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class ConnectionStatus : EventBox
    {
        private Image _connectionStatusImage;
        private Gdk.Pixbuf _pixbufStatusOn = null;
        private Gdk.Pixbuf _pixbufStatusOff = null;
        private bool _connected = false;
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        public ConnectionStatus(bool pIsConnected)
        {
            //Initialize Members
            _connected = pIsConnected;
            _pixbufStatusOn = Utils.FileToPixBuf(FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Other\connection_status_on.png"));
            _pixbufStatusOff = Utils.FileToPixBuf(FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Other\connection_status_off.png"));

            _connectionStatusImage = new Image();

            VisibleWindow = false;

            //Start Initialized
            SetStatus(_connected);

            Add(_connectionStatusImage);
        }

        public bool SetStatus(bool pIsConnected)
        {
            _connected = pIsConnected;

            if (_connected)
            {
                _connectionStatusImage.Pixbuf = _pixbufStatusOn;
            }
            else
            {
                _connectionStatusImage.Pixbuf = _pixbufStatusOff;
            }

            return _connected;
        }

        public bool ToggleStatus()
        {
            _connected = !_connected;
            return SetStatus(_connected);
        }
    }
}
