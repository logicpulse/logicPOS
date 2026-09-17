using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.InputReaders.GetAllInputReaders;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.Api.Features.PoleDisplays.GetAllPoleDisplays;
using LogicPOS.Api.Features.Printers.GetAllPrinters;
using LogicPOS.Api.Features.Terminals.UpdateTerminal;
using LogicPOS.Api.Features.WeighingMachines.GetAllWeighingMachines;
using LogicPOS.UI.Components.Terminals;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class TerminalModal: EntityEditionModal<Terminal>
    {
        public TerminalModal(EntityEditionModalMode modalMode, Terminal entity = null) : base(modalMode, entity)
        {
            _txtHardwareId.Component.Sensitive = false;
        }
        private IEnumerable<Place> GetPlaces() => ExecuteGetEntitiesQuery(new GetAllPlacesQuery());
        private IEnumerable<InputReader> GetInputReaders() => ExecuteGetEntitiesQuery(new GetAllInputReadersQuery());
        private IEnumerable<Printer> GetPrinters() => ExecuteGetEntitiesQuery(new GetAllPrintersQuery());
        private IEnumerable<PoleDisplay> GetPoleDisplays() => ExecuteGetEntitiesQuery(new GetAllPoleDisplaysQuery());
        private IEnumerable<WeighingMachine> GetWeighingMachines() => ExecuteGetEntitiesQuery(new GetAllWeighingMachinesQuery());

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtHardwareId.Entry);
            SensitiveFields.Add(_txtTimerInterval.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_checkDefaultTerminal);
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

        private UpdateTerminalCommand CreateUpdateCommand()
        {
            return new UpdateTerminalCommand
            {
                Id = _entity.Id,
                Order = uint.Parse(_txtOrder.Text),
                Code = _txtCode.Text,
                Designation = _txtDesignation.Text,
                TimerInterval = uint.Parse(_txtTimerInterval.Text),
                PlaceId = _comboPlaces.SelectedEntity?.Id,
                PrinterId = _comboPrinters.SelectedEntity?.Id,
                ThermalPrinterId = _comboThermalPrinters.SelectedEntity?.Id,
                PoleDisplayId = _comboPoleDisplays.SelectedEntity?.Id,
                WeighingMachineId = _comboWeighingMachines.SelectedEntity?.Id,
                BarcodeReaderId = _comboBarcodeReaders.SelectedEntity?.Id,
                CardReaderId = _comboCardReaders.SelectedEntity?.Id,
                IsDefault = _checkDefaultTerminal.Active,
                Notes = _txtNotes.Value.Text                
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
            _checkDefaultTerminal.Active = _entity.IsDefault;
            _checkDefaultTerminal.Sensitive = !_entity.IsDefault;
            _txtNotes.Value.Text = _entity.Notes;
        }
        protected override bool AddEntity() => false;
        protected override bool UpdateEntity() {
            var result = ExecuteUpdateCommand(CreateUpdateCommand());
            if (result.IsError)
            {
                return false;
            }
            TerminalService.RefreshTerminal();
            return true;
        }
    }
}
