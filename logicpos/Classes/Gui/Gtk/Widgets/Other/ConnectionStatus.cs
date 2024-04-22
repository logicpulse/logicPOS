using Gtk;
using logicpos.App;
using logicpos.datalayer.App;
using logicpos.shared.App;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class ConnectionStatus : EventBox
    {
        private readonly Image _connectionStatusImage;
        private readonly Gdk.Pixbuf _pixbufStatusOn = null;
        private readonly Gdk.Pixbuf _pixbufStatusOff = null;
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
            _pixbufStatusOn = logicpos.Utils.FileToPixBuf(SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Other\connection_status_on.png"));
            _pixbufStatusOff = logicpos.Utils.FileToPixBuf(SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Other\connection_status_off.png"));

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
