using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class InputReaderModal
    {

        public override Size ModalSize => new Size(500, 400);
        public override string ModalTitleResourceName => "global_ConfigurationInputReader";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private TextBox _txtReaderSizes = new TextBox("global_input_reader_card_reader_sizes", true,true, @"^\d+(,\d+)*$");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtReaderSizes.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtReaderSizes);
                    
                    break;
                case EntityModalMode.Update:
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtReaderSizes);

                    break;
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }

        private VBox CreateDetailsTab()
        {
            var tab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_txtOrder.Component, false, false, 0);
                tab1.PackStart(_txtCode.Component, false, false, 0);
            }

            tab1.PackStart(_txtDesignation.Component, false, false, 0);
            tab1.PackStart(_txtReaderSizes.Component, false, false, 0);

            if (_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }

    }
}
