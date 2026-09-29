using Gtk;
using LogicPOS.UI.Components.InputFields;
using System.Collections.Generic;

namespace LogicPOS.UI.Components
{
    public partial class SerialNumberField
    {
        public List<SerialNumberSelectionField> Children { get; private set; } = new List<SerialNumberSelectionField>();

        public TextBox TxtSerialNumber = TextBox.Simple("global_serial_number", false);
        public VBox Component { get; private set; } = new VBox(false, 2);
    }
}
