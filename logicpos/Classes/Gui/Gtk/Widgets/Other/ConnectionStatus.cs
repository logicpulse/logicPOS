using Gtk;
using logicpos.datalayer.App;
using logicpos.shared.App;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public class ConnectionStatus : EventBox
    {
        private readonly Image _connectionStatusImage;
        private readonly Gdk.Pixbuf _pixbufStatusOn = null;
        private readonly Gdk.Pixbuf _pixbufStatusOff = null;

        public bool Connected { get; set; } = false;

        public ConnectionStatus(bool pIsConnected)
        {
            //Initialize Members
            Connected = pIsConnected;
            _pixbufStatusOn = logicpos.Utils.FileToPixBuf(SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Other\connection_status_on.png"));
            _pixbufStatusOff = logicpos.Utils.FileToPixBuf(SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Other\connection_status_off.png"));

            _connectionStatusImage = new Image();

            VisibleWindow = false;

            //Start Initialized
            SetStatus(Connected);

            Add(_connectionStatusImage);
        }

        public bool SetStatus(bool pIsConnected)
        {
            Connected = pIsConnected;

            if (Connected)
            {
                _connectionStatusImage.Pixbuf = _pixbufStatusOn;
            }
            else
            {
                _connectionStatusImage.Pixbuf = _pixbufStatusOff;
            }

            return Connected;
        }

        public bool ToggleStatus()
        {
            Connected = !Connected;
            return SetStatus(Connected);
        }
    }
}
