using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Printer = LogicPOS.Api.Entities.Printer;

namespace LogicPOS.UI.Components.Modals
{
    public partial class TerminalModal
    {
        public override Size ModalSize => new Size(500, 550);
        public override string ModalTitleResourceName => "dialog_edit_ConfigurationPlaceTerminal_tab1_label";

        protected override void BeforeDesign()
        {
            InitializePlacesComboBox();
            InitializePoleDisplaysComboBox();
            InitializePrintersComboBox();
            InitializeThermalPrintersComboBox();
            InitializeWeighingMachinesComboBox();
            InitializeBarcodeReadersComboBox();
            InitializeCardReadersComboBox();
        }

        private void InitializePlacesComboBox()
        {
            var places = GetPlaces();
            var labelText = GeneralUtils.GetResourceByName("global_places");
            var currentPlace = _entity != null ? _entity.Place : null;

            _comboPlaces = new EntityComboBox<Place>(labelText,
                                                             places,
                                                             currentPlace,
                                                             false);
        }

        private void InitializePrintersComboBox()
        {
            var printers = GetPrinters();
            var labelText = GeneralUtils.GetResourceByName("global_printers");
            var currentPrinter = _entity != null ? _entity.Printer : null;

            _comboPrinters = new EntityComboBox<Printer>(labelText,
                                                             printers,
                                                             currentPrinter,
                                                             false);
        }

        private void InitializeThermalPrintersComboBox()
        {
            var printers = GetPrinters().Where(p => p.Type.ThermalPrinter);

            var labelText = GeneralUtils.GetResourceByName("global_printer_thermal_printer");
            var currentPrinter = _entity != null ? _entity.ThermalPrinter : null;

            _comboThermalPrinters = new EntityComboBox<Printer>(labelText,
                                                             printers,
                                                             currentPrinter,
                                                             false);
        }

        private void InitializePoleDisplaysComboBox()
        {
            var poleDisplays = GetPoleDisplays();
            var labelText = GeneralUtils.GetResourceByName("global_ConfigurationPoleDisplay");
            var currentPoleDisplay = _entity != null ? _entity.PoleDisplay : null;

            _comboPoleDisplays = new EntityComboBox<PoleDisplay>(labelText,
                                                             poleDisplays,
                                                             currentPoleDisplay,
                                                             false);
        }

        private void InitializeWeighingMachinesComboBox()
        {
            var weighingMachines = GetWeighingMachines();
            var labelText = GeneralUtils.GetResourceByName("global_ConfigurationWeighingMachine");
            var currentWeighingMachine = _entity != null ? _entity.WeighingMachine : null;

            _comboWeighingMachines = new EntityComboBox<WeighingMachine>(labelText,
                                                             weighingMachines,
                                                             currentWeighingMachine,
                                                             false);
        }
        private void InitializeBarcodeReadersComboBox()
        {
            var barcodeReaders = GetInputReaders();
            var labelText = GeneralUtils.GetResourceByName("global_input_barcode_reader");
            var currentBarcodeReader = _entity != null ? _entity.BarcodeReader : null;

            _comboBarcodeReaders = new EntityComboBox<InputReader>(labelText,
                                                             barcodeReaders,
                                                             currentBarcodeReader,
                                                             false);
        }

        private void InitializeCardReadersComboBox()
        {
            var cardReaders = GetInputReaders();
            var labelText = GeneralUtils.GetResourceByName("global_input_reader_card_reader");
            var currentCardReader = _entity != null ? _entity.CardReader : null;

            _comboCardReaders = new EntityComboBox<InputReader>(labelText,
                                                             cardReaders,
                                                             currentCardReader,
                                                             false);
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateDevicesTab(), GeneralUtils.GetResourceByName("global_devices"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }

        private VBox CreateDetailsTab()
        {
            var tab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_txtOrder.Component, false, false, 0);
                tab1.PackStart(_txtCode.Component, false, false, 0);

            }

            tab1.PackStart(_txtDesignation.Component, false, false, 0);
            tab1.PackStart(_txtHardwareId.Component, false, false, 0);
            tab1.PackStart(_comboPlaces.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }

        private VBox CreateDevicesTab()
        {
            var tab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab2.PackStart(_comboPrinters.Component, false, false, 0);
            tab2.PackStart(_comboThermalPrinters.Component, false,false, 0);
            tab2.PackStart(_comboPoleDisplays.Component, false, false, 0);
            tab2.PackStart(_comboWeighingMachines.Component, false, false, 0);
            tab2.PackStart(_comboBarcodeReaders.Component, false, false, 0);
            tab2.PackStart(_comboCardReaders.Component, false, false, 0);
            tab2.PackStart(_txtTimerInterval.Component, false, false, 0);

            return tab2;
        }
    }
}
