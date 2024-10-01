
using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;

namespace LogicPOS.UI.Components.Modals.Common
{
    public abstract class ModalTab : Box, IValidatableField
    {
        public string PageName { get; set; }
        public string PageIcon { get; set; }
        public Window SourceWindow { get; set; }
        public bool Enabled { get; set; } = true;
        public IconButtonWithText BtnNavigator { get; set; }

        public string FieldName => PageName;

        public ModalTab(Window parent,
                         string name,
                         string icon,
                         bool enabled = true)
        {
            SourceWindow = parent;
            PageName = name;
            PageIcon = icon;
            Enabled = enabled;
        }

        public abstract bool IsValid();     
    }
}
