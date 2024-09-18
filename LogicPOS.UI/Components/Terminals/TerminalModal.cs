using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.InputReaders.GetAllInputReaders;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.Api.Features.PoleDisplays.GetAllPoleDisplays;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.Api.Features.Terminals.AddTerminal;
using LogicPOS.Api.Features.Terminals.UpdateTerminal;
using LogicPOS.Api.Features.WeighingMachines.GetAllWeighingMachines;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class TerminalModal: EntityEditionModal<Terminal>
    {
        public TerminalModal(EntityEditionModalMode modalMode, Terminal entity = null) : base(modalMode, entity)
        {
        }


        private IEnumerable<Place> GetPlaces() => ExecuteGetAllQuery(new GetAllPlacesQuery());
        private IEnumerable<InputReader> GetInputReaders() => ExecuteGetAllQuery(new GetAllInputReadersQuery());
        private IEnumerable<Printer> GetPrinters() => ExecuteGetAllQuery(new GetAllPrintersQuery());
        private IEnumerable<PoleDisplay> GetPoleDisplays() => ExecuteGetAllQuery(new GetAllPoleDisplaysQuery());
        private IEnumerable<WeighingMachine> GetWeighingMachines() => ExecuteGetAllQuery(new GetAllWeighingMachinesQuery());

        private AddTerminalCommand CreateAddCommand()
        {
            return new AddTerminalCommand
            {
                Designation = _txtDesignation.Text,
                HardwareId = _txtHardwareId.Text,
                TimerInterval = uint.Parse(_txtTimerInterval.Text),
                PlaceId= _comboPlaces.SelectedEntity?.Id,
                PrinterId= _comboPrinters.SelectedEntity?.Id,
                ThermalPrinterId= _comboThermalPrinters.SelectedEntity?.Id,
                PoleDisplayId= _comboPoleDisplays.SelectedEntity?.Id,
                WeighingMachineId= _comboWeighingMachines.SelectedEntity?.Id,
                BarcodeReaderId= _comboBarcodeReaders.SelectedEntity?.Id,
                CardReaderId= _comboCardReaders.SelectedEntity?.Id,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateTerminalCommand CreateUpdateCommand()
        {
            return new UpdateTerminalCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewTimerInterval = uint.Parse(_txtTimerInterval.Text),
                NewPlaceId = _comboPlaces.SelectedEntity.Id,
                NewPrinterId = _comboPrinters.SelectedEntity.Id,
                NewThermalPrinterId = _comboThermalPrinters.SelectedEntity.Id,
                NewPoleDisplayId = _comboPoleDisplays.SelectedEntity.Id,
                NewWeighingMachineId = _comboWeighingMachines.SelectedEntity.Id,
                NewBarcodeReaderId = _comboBarcodeReaders.SelectedEntity.Id,
                NewCardReaderId = _comboCardReaders.SelectedEntity.Id,
                NewNotes = _txtNotes.Value.Text                
            };
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtTimerInterval.Text = _entity.TimerInterval.ToString();
            _txtHardwareId.Text = _entity.HardwareId;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }
        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());
        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

    }
}
