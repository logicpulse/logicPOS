using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class TerminalModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtHardwareId = TextBox.Simple("global_hardware_id", true, true, @".+");
        private TextBox _txtTimerInterval = TextBox.Simple("global_input_reader_timer_interval", true, true, @"^\d*$");
        private EntityComboBox<Place> _comboPlaces;
        private EntityComboBox<Api.Entities.Printer> _comboPrinters;
        private EntityComboBox<Api.Entities.Printer> _comboThermalPrinters;
        private EntityComboBox<PoleDisplay> _comboPoleDisplays;
        private EntityComboBox<WeighingMachine> _comboWeighingMachines;
        private EntityComboBox<InputReader> _comboBarcodeReaders;
        private EntityComboBox<InputReader> _comboCardReaders;
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
     
    }
}
