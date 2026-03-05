using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PrinterModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtNetworkName = TextBox.Simple("global_networkname");
        private ComboBox _comboDesignation;
        private Label _labelDesignation => CreateDesignationLabel("global_designation");
        private EntityComboBox<PrinterType> _comboPrinterTypes;
    }
}
