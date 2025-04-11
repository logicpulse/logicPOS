using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PrinterTypeModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtToken = TextBox.Simple("global_DialogConfigurationPrintersTypetoken", true);
        private CheckButton _checkThermalPrinter = new CheckButton(GeneralUtils.GetResourceByName("global_printer_thermal_printer"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
