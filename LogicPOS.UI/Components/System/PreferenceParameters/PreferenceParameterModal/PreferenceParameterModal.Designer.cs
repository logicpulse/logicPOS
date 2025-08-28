using System;
using System.Collections.Generic;
using System.Drawing;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;


namespace LogicPOS.UI.Components.Modals
{
    public partial class PreferenceParameterModal
    {
        public override Size ModalSize => new Size(500, 500);
        public override string ModalTitleResourceName => "window_title_edit_configurationpreferenceparameter";

        protected override void BeforeDesign()
        {
            if (_modalMode != EntityEditionModalMode.Insert)
            {
                InitializeField();
            }
        }
        private void InitializeField()
        {
            _field = new PreferenceParameterInputField(_entity as PreferenceParameter);
        }
        protected override void AddSensitiveFields()
        {       
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_field.FieldComponent);
        }
        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_txtOrder);
            ValidatableFields.Add(_txtCode);
            ValidatableFields.Add(_field.TextBox);
        }
        private VBox CreateDetailsTab()
        {
            var details = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            details.PackStart(_txtOrder.Component, false, false, 0);
            details.PackStart(_txtCode.Component, false, false, 0);
            details.PackStart(_txtToken.Component, false, false, 0);
            details.PackStart(_field.FieldComponent, false, false, 0);
            return details;
        }
        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }
    }
}
