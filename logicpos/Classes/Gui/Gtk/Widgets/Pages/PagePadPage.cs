using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.shared.App;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    /// <summary>
    /// Class for pages used in PagePad
    /// </summary>
    internal abstract class PagePadPage : Box
    {
        //Log4Net
        protected log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Fields

        //Protected

        //Public Fields
        private string _pageName;
        public string PageName
        {
            get { return _pageName; }
            set { _pageName = value; }
        }

        private string _pageIcon;
        public string PageIcon
        {
            get { return _pageIcon; }
            set { _pageIcon = value; }
        }

        protected Window _sourceWindow;
        public Window SourceWindow
        {
            get { return _sourceWindow; }
            set { _sourceWindow = value; }
        }

        protected bool _validated = false;
        public bool Validated
        {
            get { return _validated; }
            set { _validated = value; }
        }

        private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        //Reference to Navigator Button
        private TouchButtonIconWithText _navigatorButton;
        public TouchButtonIconWithText NavigatorButton
        {
            get { return _navigatorButton; }
            set { _navigatorButton = value; }
        }

        //Required to Override Public Methods
        public abstract void Validate();

        //Constructors
        public PagePadPage(Window pSourceWindow, string pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget) { }
        public PagePadPage(Window pSourceWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _pageName = pPageName;
            _pageIcon = (pPageIcon != string.Empty && File.Exists(pPageIcon))
              ? pPageIcon
              //DefaultIcon
              : SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/icon_pos_default.png"));
            if (pWidget != null) PackStart(pWidget);
            _enabled = pEnabled;
        }
    }
}
