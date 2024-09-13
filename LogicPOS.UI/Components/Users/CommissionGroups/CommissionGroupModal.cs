using DocumentFormat.OpenXml.ExtendedProperties;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.CommissionGroups.AddCommissionGroup;
using LogicPOS.Api.Features.CommissionGroups.UpdateCommissionGroup;
using LogicPOS.UI.Components.Modals;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CommissionGroupModal: EntityEditionModal<CommissionGroup>
    {
        public CommissionGroupModal(EntityEditionModalMode modalMode, CommissionGroup commissionGroup = null) : base(modalMode, commissionGroup)
        {
        }

        private AddCommissionGroupCommand CreateAddCommand()
        {
            return new AddCommissionGroupCommand
            {
                Designation = _txtDesignation.Text,
                Commission = decimal.Parse(_txtCommission.Text),
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateCommissionGroupCommand CreateUpdateCommand()
        {
            return new UpdateCommissionGroupCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewCommission = decimal.Parse(_txtCommission.Text),
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
            _txtCommission.Text = _entity.Commission.ToString();
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text= _entity.Notes;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

    }
}