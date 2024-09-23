
using Gtk;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components.Modals.Common
{
    public abstract class ModalPage : Box
    {
        public string PageName { get; set; }
        public string PageIcon { get; set; }
        public Window SourceWindow { get; set; }
        public bool Enabled { get; set; } = true;
        public IconButtonWithText BtnNavigator { get; set; }

        public ModalPage(Window parent,
                         string name,
                         string icon,
                         bool enabled = true)
        {
            SourceWindow = parent;
            PageName = name;
            PageIcon = icon;
            Enabled = enabled;
        }

    }
}
