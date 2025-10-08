using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatExemptionReasons.AddVatExemptionReason;
using LogicPOS.Api.Features.VatExemptionReasons.UpdateVatExemptionReason;

namespace LogicPOS.UI.Components.Modals
{
    public partial class VatExemptionReasonModal : EntityEditionModal<VatExemptionReason>
    {
        public VatExemptionReasonModal(EntityEditionModalMode modalMode, VatExemptionReason entity = null) : base(modalMode, entity)
        {
        }

        private AddVatExemptionReasonCommand CreateAddCommand()
        {
            return new AddVatExemptionReasonCommand
            {
                Designation = _txtDesignation.Text,
                Acronym = _txtAcronym.Text,
                StandardApplicable = _txtStandardApplicable.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateVatExemptionReasonCommand CreateUpdateCommand()
        {
            return new UpdateVatExemptionReasonCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewAcronym = _txtAcronym.Text,
                NewStandardApplicable = _txtStandardApplicable.Text,
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
            _txtAcronym.Text = _entity.Acronym;
            _txtStandardApplicable.Text = _entity.StandardApplicable;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;
    }
}
