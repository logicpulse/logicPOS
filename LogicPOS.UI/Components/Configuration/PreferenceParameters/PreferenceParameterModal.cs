using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PreferenceParameters.UpdatePreferenceParameter;
using System;
using System.IO;

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

        public string logoGetBase64Image()
        {
            if (_entity.Token.Contains("LOGO"))
            {
                if (string.IsNullOrWhiteSpace(_field.TextBox.Text))
                {
                    return null;
                }

                var result = Convert.ToBase64String(File.ReadAllBytes(_field.TextBox.Text));
                return result;
            }
            return _field.TextBox.Text;
        }
        private UpdatePreferenceParameterCommand CreateUpdateCommand()
        {
            ;
            return new UpdatePreferenceParameterCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewValue = logoGetBase64Image(),
                NewNotes = _txtNotes.Value.Text
            };
        }
    }
}
