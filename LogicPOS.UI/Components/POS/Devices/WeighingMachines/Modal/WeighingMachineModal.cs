
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.WeighingMachines.AddWeighingMachine;
using LogicPOS.Api.Features.WeighingMachines.UpdateWeighingMachine;


namespace LogicPOS.UI.Components.Modals
{
    public partial class WeighingMachineModal : EntityEditionModal<WeighingMachine>
    {
        public WeighingMachineModal(EntityEditionModalMode modalMode, WeighingMachine weighingMachine = null) : base(modalMode, weighingMachine)
        {
        }

        private AddWeighingMachineCommand CreateAddCommand()
        {
            return new AddWeighingMachineCommand
            {
                Designation = _txtDesignation.Text,
                PortName = _txtPortName.Text,
                BaudRate = uint.Parse(_txtBaudRate.Text),
                Parity = _txtParity.Text,
                StopBits = _txtStopBits.Text,
                DataBits = uint.Parse(_txtDataBits.Text),
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateWeighingMachineCommand CreateUpdateCommand()
        {
            return new UpdateWeighingMachineCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewPortName = _txtPortName.Text,
                NewBaudRate = uint.Parse(_txtBaudRate.Text),
                NewParity = _txtParity.Text,
                NewStopBits = _txtStopBits.Text,
                NewDataBits = uint.Parse(_txtDataBits.Text),
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;
 
        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtPortName.Text = _entity.PortName;
            _txtBaudRate.Text = _entity.BaudRate.ToString();
            _txtParity.Text = _entity.Parity;
            _txtStopBits.Text = _entity.StopBits;
            _txtDataBits.Text = _entity.DataBits.ToString();
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text= _entity.Notes;
        }

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

    }
}