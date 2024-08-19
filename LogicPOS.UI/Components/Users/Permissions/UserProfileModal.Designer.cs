using System;
using System.Drawing;
using Gtk;
using LogicPOS.UI.Components.InputFieds;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserProfileModal
    {
        public override Size ModalSize => new Size(500, 335);
        public override string ModalTitleResourceName => "window_title_edit_user_profile";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void Design()
        {
            var notebook = CreateNoteBook();
            VBox.PackStart(notebook, true, true, 0);
        }

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {
            switch(_modalMode)
            {
               case EntityModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    break;
                case EntityModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    break;
            }
        }
        
        private VBox CreateTab1()
        {
            var tab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_txtOrder.Component, false, false, 0);
                tab1.PackStart(_txtCode.Component, false, false, 0);
            }

            tab1.PackStart(_txtDesignation.Component, false, false, 0);

            if(_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

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
