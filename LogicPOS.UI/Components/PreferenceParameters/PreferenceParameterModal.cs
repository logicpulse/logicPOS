using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PreferenceParameters.UpdatePreferenceParameter;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PreferenceParameterModal : EntityModal
    {
        public PreferenceParameterModal(EntityModalMode modalMode,
                                        PreferenceParameter entity = null) : base(modalMode, entity)
        {
        }


        protected override void AddEntity()
        {
            throw new InvalidOperationException();
        }

        protected override void ShowEntityData()
        {
            var preferenceParameter = _entity as PreferenceParameter;
            _txtOrder.Text = preferenceParameter.Order.ToString();
            _txtCode.Text = preferenceParameter.Code;
            _txtToken.Text = preferenceParameter.Token;
            _txtNotes.Value.Text = preferenceParameter.Notes;
        }

        protected override void UpdateEntity()
        {
            var command = CreateUpdateCommand();
            var result = _mediator.Send(command).Result;
        
            if (result.IsError)
            {
                HandleApiError(result.FirstError);
            }
        }

        private UpdatePreferenceParameterCommand CreateUpdateCommand()
        {
            return new UpdatePreferenceParameterCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewValue = _field.GetValue(),
                NewNotes = _txtNotes.Value.Text
            };
        }
    }
}
