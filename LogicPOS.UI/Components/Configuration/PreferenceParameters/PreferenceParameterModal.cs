using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PreferenceParameters.UpdatePreferenceParameter;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PreferenceParameterModal : EntityEditionModal<PreferenceParameter>
    {
        public PreferenceParameterModal(EntityEditionModalMode modalMode,
                                        PreferenceParameter preferenceParameter = null) : base(modalMode, preferenceParameter)
        {
            _txtToken.Entry.Sensitive = false;
        }


        protected override void AddEntity() => throw new InvalidOperationException();

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtToken.Text = _entity.Token;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private UpdatePreferenceParameterCommand CreateUpdateCommand()
        {
            return new UpdatePreferenceParameterCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewValue = _field.TextBox.Text,
                NewNotes = _txtNotes.Value.Text
            };
        }
    }
}
