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
        private TextBox _txtThermalMaxCharsPerLineNormal = TextBox.Simple("global_printer_thermal_max_chars_per_line_normal", true, true, "^[0-9]+$");
        private TextBox _txtThermalMaxCharsPerLineNormalBold = TextBox.Simple("global_printer_thermal_max_chars_per_line_normal_bold", true, true, "^[0-9]+$");
        private TextBox _txtThermalMaxCharsPerLineSmall = TextBox.Simple("global_printer_thermal_max_chars_per_line_small", true, true, "^[0-9]+$");
        private ComboBox _comboDesignation;
        private Label _labelDesignation => CreateDesignationLabel("global_designation");
        private EntityComboBox<PrinterType> _comboPrinterTypes;
    }
}
