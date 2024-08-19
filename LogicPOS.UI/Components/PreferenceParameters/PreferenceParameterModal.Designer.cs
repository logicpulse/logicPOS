using System;
using System.Drawing;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFieds;
using LogicPOS.Utility;


namespace LogicPOS.UI.Components.Modals
{
    public partial class PreferenceParameterModal
    {
        public override Size ModalSize => new Size(500, 500);
        public override string ModalTitleResourceName => "window_title_edit_configurationpreferenceparameter";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtToken = new TextBox("global_token");
        private PreferenceParameterInputField _field;
        #endregion

        protected override void Design()
        {
            _txtToken.Entry.Sensitive = false;
            var notebook = CreateNoteBook();
            VBox.PackStart(notebook, true, true, 0);
        }

        private void InitializeField()
        {
            _field = new PreferenceParameterInputField(_entity as PreferenceParameter);
        }

        protected override void AddSensitiveFields()
        {
            if(_field == null)
            {
                InitializeField();
            }
            
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_field.FieldComponent);
        }

        protected override void AddValidatableFields()
        {
            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    throw new NotImplementedException();
                case EntityModalMode.Update:
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    break;
            }
        }

        private VBox CreateTab1()
        {
            var tab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab1.PackStart(_txtOrder.Component, false, false, 0);
            tab1.PackStart(_txtCode.Component, false, false, 0);
            tab1.PackStart(_txtToken.Component, false, false, 0);
            tab1.PackStart(_field.FieldComponent, false, false, 0);
            return tab1;
        }

        private Notebook CreateNoteBook()
        {
            Notebook notebook = new Notebook();
            notebook.BorderWidth = 3;

            notebook.AppendPage(CreateTab1(), new Label(GeneralUtils.GetResourceByName("global_record_main_detail")));
            notebook.AppendPage(CreateNotesTab(), new Label(GeneralUtils.GetResourceByName("global_notes")));
            return notebook;
        }
    }
}
