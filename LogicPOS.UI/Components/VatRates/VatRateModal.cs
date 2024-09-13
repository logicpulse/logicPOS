using DevExpress.Schedule;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatRates.AddVatRate;
using LogicPOS.Api.Features.VatRates.UpdateVatRate;
using System.Globalization;

namespace LogicPOS.UI.Components.Modals
{
    public partial class VatRateModal: EntityEditionModal<VatRate>
    {
        public VatRateModal(EntityEditionModalMode modalMode, VatRate entity = null) : base(modalMode, entity)
        {
        }

        private AddVatRateCommand CreateAddCommand()
        {
            return new AddVatRateCommand
            {
                Designation = _txtDesignation.Text,
                Value = decimal.Parse(_txtValue.Text),
                TaxType = _txtTaxType.Text,
                TaxCode = _txtTaxCode.Text,
                CountryRegionCode = _txtCountryRegionCode.Text,
                Description = _txtDescription.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateVatRateCommand CreateUpdateCommand()
        {
            return new UpdateVatRateCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewValue = decimal.Parse(_txtValue.Text),
                NewTaxType = _txtTaxType.Text,
                NewTaxCode = _txtTaxCode.Text,
                NewCountryRegionCode = _txtCountryRegionCode.Text,
                NewDescription = _txtDescription.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtValue.Text = _entity.Value.ToString();
            _txtTaxType.Text = _entity.TaxType;
            _txtTaxCode.Text= _entity.TaxCode;
            _txtCountryRegionCode.Text= _entity.CountryRegion;
            _txtDescription.Text = _entity.Description;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }
        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());
        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

    }
}
