using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
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

        public string PageName { get; set; }

        public string PageIcon { get; set; }

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

        public bool Enabled { get; set; } = true;

        public IconButtonWithText NavigatorButton { get; set; }

        //Required to Override Public Methods
        public abstract void Validate();

        //Constructors
        public PagePadPage(Window parentWindow, string pPageName, Widget pWidget) : this(parentWindow, pPageName, "", pWidget) { }
        public PagePadPage(Window parentWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
        {
            //Parameters
            _sourceWindow = parentWindow;
            PageName = pPageName;
            PageIcon = (pPageIcon != string.Empty && File.Exists(pPageIcon))
              ? pPageIcon
              //DefaultIcon
              : string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, @"Icons/icon_pos_default.png");
            if (pWidget != null) PackStart(pWidget);
            Enabled = pEnabled;
        }
    }
}
