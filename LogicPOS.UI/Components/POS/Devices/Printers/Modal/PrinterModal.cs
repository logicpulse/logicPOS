using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Printers.AddPrinter;
using LogicPOS.Api.Features.Printers.UpdatePrinter;
using LogicPOS.Api.Features.PrinterTypes.GetAllPrinterTypes;
using LogicPOS.UI.Components.InputFields;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PrinterModal : EntityEditionModal<Printer>
    {
        public PrinterModal(EntityEditionModalMode modalMode, Printer entity = null) : base(modalMode, entity)
        {
        }

        private IEnumerable<PrinterType> GetPrinterTypes() => ExecuteGetEntitiesQuery(new GetAllPrinterTypesQuery());

        protected override void ShowEntityData()
        {
            _txtCode.Text = _entity.Code;
            _txtOrder.Text = _entity.Order.ToString();
            _txtNetworkName.Text = _entity.NetworkName;
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;

            _txtThermalMaxCharsPerLineNormal.Text = (_entity.ThermalMaxCharsPerLineNormal.GetValueOrDefault() > 0
                ? _entity.ThermalMaxCharsPerLineNormal.Value
                : Printer.DefaultThermalMaxCharsPerLineNormal).ToString();

            _txtThermalMaxCharsPerLineNormalBold.Text = (_entity.ThermalMaxCharsPerLineNormalBold.GetValueOrDefault() > 0
                ? _entity.ThermalMaxCharsPerLineNormalBold.Value
                : Printer.DefaultThermalMaxCharsPerLineNormalBold).ToString();

            _txtThermalMaxCharsPerLineSmall.Text = (_entity.ThermalMaxCharsPerLineSmall.GetValueOrDefault() > 0
                ? _entity.ThermalMaxCharsPerLineSmall.Value
                : Printer.DefaultThermalMaxCharsPerLineSmall).ToString();
        }

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

        private int ParseThermalChars(TextBox field, int defaultValue)
        {
            return int.TryParse(field.Text, out var value) && value > 0 ? value : defaultValue;
        }

        private UpdatePrinterCommand CreateUpdateCommand()
        {
            var isThermal = IsThermalTypeSelected();
            return new UpdatePrinterCommand
            {
                Id = _entity.Id,
                Order = uint.Parse(_txtOrder.Text),
                Code = _txtCode.Text,
                Designation = _comboDesignation.ActiveText,
                Notes = _txtNotes.Value.Text,
                NetworkName = _txtNetworkName.Text,
                IsDeleted = _checkDisabled.Active,
                TypeId = _comboPrinterTypes.SelectedEntity.Id,
                ThermalMaxCharsPerLineNormal = isThermal
                    ? ParseThermalChars(_txtThermalMaxCharsPerLineNormal, Printer.DefaultThermalMaxCharsPerLineNormal)
                    : Printer.DefaultThermalMaxCharsPerLineNormal,
                ThermalMaxCharsPerLineNormalBold = isThermal
                    ? ParseThermalChars(_txtThermalMaxCharsPerLineNormalBold, Printer.DefaultThermalMaxCharsPerLineNormalBold)
                    : Printer.DefaultThermalMaxCharsPerLineNormalBold,
                ThermalMaxCharsPerLineSmall = isThermal
                    ? ParseThermalChars(_txtThermalMaxCharsPerLineSmall, Printer.DefaultThermalMaxCharsPerLineSmall)
                    : Printer.DefaultThermalMaxCharsPerLineSmall
            };
        }

        private AddPrinterCommand CreateAddCommand()
        {
            var isThermal = IsThermalTypeSelected();
            return new AddPrinterCommand
            {
                Designation = _comboDesignation.ActiveText,
                NetworkName = _txtNetworkName.Text,
                Notes = _txtNotes.Value.Text,
                TypeId = _comboPrinterTypes.SelectedEntity.Id,
                ThermalMaxCharsPerLineNormal = isThermal
                    ? ParseThermalChars(_txtThermalMaxCharsPerLineNormal, Printer.DefaultThermalMaxCharsPerLineNormal)
                    : Printer.DefaultThermalMaxCharsPerLineNormal,
                ThermalMaxCharsPerLineNormalBold = isThermal
                    ? ParseThermalChars(_txtThermalMaxCharsPerLineNormalBold, Printer.DefaultThermalMaxCharsPerLineNormalBold)
                    : Printer.DefaultThermalMaxCharsPerLineNormalBold,
                ThermalMaxCharsPerLineSmall = isThermal
                    ? ParseThermalChars(_txtThermalMaxCharsPerLineSmall, Printer.DefaultThermalMaxCharsPerLineSmall)
                    : Printer.DefaultThermalMaxCharsPerLineSmall
            };
        }
        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;

    }
}
