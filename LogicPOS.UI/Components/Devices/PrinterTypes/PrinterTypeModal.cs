
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PrinterTypes.AddPrinterType;
using LogicPOS.Api.Features.PrinterTypes.UpdatePrinterType;


namespace LogicPOS.UI.Components.Modals
{
    public partial class PrinterTypeModal : EntityEditionModal<PrinterType>
    {
        public PrinterTypeModal(EntityEditionModalMode modalMode, PrinterType printerType = null) : base(modalMode, printerType)
        {
        }

        private AddPrinterTypeCommand CreateAddCommand()
        {
            return new AddPrinterTypeCommand
            {
                Designation = _txtDesignation.Text,
                Token = _txtToken.Text,
                ThermalPrinter = _checkThermalPrinter.Active,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdatePrinterTypeCommand CreateUpdateCommand()
        {
            return new UpdatePrinterTypeCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewToken = _txtToken.Text,
                ThermalPrinter = _checkThermalPrinter.Active,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());
 
        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtToken.Text = _entity.Token;
            _checkThermalPrinter.Active = _entity.ThermalPrinter;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text= _entity.Notes;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

    }
}