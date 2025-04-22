using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WeighingMachineModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtPortName = TextBox.Simple("global_hardware_com_portname", true, true, @"^COM\d+$");
        private TextBox _txtBaudRate = TextBox.Simple("global_hardware_com_baudrate", true, true, @"^(4800|9600)$");
        private TextBox _txtParity = TextBox.Simple("global_hardware_com_parity", true, true, @"^(Even|None|Odd)$");
        private TextBox _txtStopBits = TextBox.Simple("global_hardware_com_stopbits", true, true, @"^(One|Two)$");
        private TextBox _txtDataBits = TextBox.Simple("global_hardware_com_databits", true, true, @"^(7|8|9)$");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
