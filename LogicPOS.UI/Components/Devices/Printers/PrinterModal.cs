using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Printers.AddPrinter;
using LogicPOS.Api.Features.Printers.UpdatePrinter;
using LogicPOS.Api.Features.PrinterTypes.GetAllPrinterTypes;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PrinterModal : EntityModal<Printer>
    {
        public PrinterModal(EntityModalMode modalMode, Printer entity = null) : base(modalMode, entity)
        {
        }

        private IEnumerable<PrinterType> GetPrinterTypes()
        {
            var getPrinterTypesResult = _mediator.Send(new GetAllPrinterTypesQuery()).Result;

            if (getPrinterTypesResult.IsError)
            {
                return Enumerable.Empty<PrinterType>();
            }

            return getPrinterTypesResult.Value;
        }

        protected override void ShowEntityData()
        {
            _txtCode.Text = _entity.Code;
            _txtOrder.Text = _entity.Order.ToString();
            _txtNetworkName.Text = _entity.NetworkName;
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());


        private UpdatePrinterCommand CreateUpdateCommand()
        {
            return new UpdatePrinterCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _comboDesignation.ActiveText,
                NewNotes = _txtNotes.Value.Text,
                NewNetworkName = _txtNetworkName.Text,
                IsDeleted = _checkDisabled.Active,
                NewTypeId = _comboPrinterTypes.SelectedEntity.Id
            };
        }

        private AddPrinterCommand CreateAddCommand()
        {
            return new AddPrinterCommand
            {
                Designation = _comboDesignation.ActiveText,
                NetworkName = _txtNetworkName.Text,
                Notes = _txtNotes.Value.Text,
                TypeId = _comboPrinterTypes.SelectedEntity.Id
            };
        }

       

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());


    }
}
