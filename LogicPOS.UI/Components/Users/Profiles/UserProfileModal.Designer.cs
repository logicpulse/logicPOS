using System;
using System.Drawing;
using Gtk;
using LogicPOS.UI.Components.InputFieds;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserProfileModal
    {
        #region Components
        private TextBox _txtOrder = new TextBox("global_record_order", true);
        private TextBox _txtCode = new TextBox("global_record_code", true);
        private TextBox _txtDesignation = new TextBox("global_designation", true);
        private MultilineTextBox _txtNotes = new MultilineTextBox();
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        public override Size ModalSize => new Size(500, 335);
        public override string ModalTitleResourceName => "window_title_edit_user_profile";

        protected override void Design()
        {
            var notebook = CreateNoteBook();
            VBox.PackStart(notebook, true, true, 0);
        }

        protected override void RegisterFields()
        {
            _fields.Add(_txtOrder.Entry);
            _fields.Add(_txtCode.Entry);
            _fields.Add(_txtDesignation.Entry);
            _fields.Add(_txtNotes.TextView);
            _fields.Add(_checkDisabled);
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

        private VBox CreateNotesTab()
        {
            VBox box = new VBox(true, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            _txtNotes.ScrolledWindow.BorderWidth = 0;
            box.PackStart(_txtNotes, true, true, 0);
            return box;
        }

    }
}
