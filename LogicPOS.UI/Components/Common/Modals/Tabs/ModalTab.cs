
using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;

namespace LogicPOS.UI.Components.Modals.Common
{
    public abstract class ModalTab : Box, IValidatableField
    {
        public string TabName { get; set; }
        public string TabIcon { get; set; }
        public Window SourceWindow { get; set; }
        public IconButtonWithText Button { get; set; }
        public string FieldName => TabName;
        public bool ShowTab { get; set; }
        public int Index { get; set; }

        public ModalTab(Window parent,
                         string name,
                         string icon,
                         bool showTab = true)
        {
            SourceWindow = parent;
            TabName = name;
            TabIcon = icon;
            ShowTab = showTab;
        }

        public abstract bool IsValid();     
    }
}
