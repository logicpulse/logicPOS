using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    /// <summary>
    /// Class for pages used in PagePad
    /// </summary>
    abstract class PagePadPage : Box
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Fields

        //Protected

        //Public Fields
        private String _pageName;
        public String PageName
        {
            get { return _pageName; }
            set { _pageName = value; }
        }

        private String _pageIcon;
        public String PageIcon
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
        public PagePadPage(Window pSourceWindow, String pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget) { }
        public PagePadPage(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _pageName = pPageName;
            _pageIcon = (pPageIcon != string.Empty && File.Exists(pPageIcon))
              ? pPageIcon
                //DefaultIcon
              : FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], @"Icons/icon_pos_default.png"));
            if (pWidget != null) PackStart(pWidget);
            _enabled = pEnabled;
        }
    }
}
