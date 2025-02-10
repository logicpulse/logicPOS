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

        #region Components
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtHardwareId = TextBox.Simple("global_hardware_id", true, true, @".+");
        private TextBox _txtTimerInterval = TextBox.Simple("global_input_reader_timer_interval", true, true, @"^\d*$");
        private EntityComboBox<Place> _comboPlaces;
        private EntityComboBox<Printer> _comboPrinters;
        private EntityComboBox<Printer> _comboThermalPrinters;
        private EntityComboBox<PoleDisplay> _comboPoleDisplays;
        private EntityComboBox<WeighingMachine> _comboWeighingMachines;
        private EntityComboBox<InputReader> _comboBarcodeReaders;
        private EntityComboBox<InputReader> _comboCardReaders;

        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion


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

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtHardwareId.Entry);
            SensitiveFields.Add(_txtTimerInterval.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtHardwareId);
                    ValidatableFields.Add(_txtTimerInterval);


                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtHardwareId);
                    ValidatableFields.Add(_txtTimerInterval);

                    break;
            }
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
