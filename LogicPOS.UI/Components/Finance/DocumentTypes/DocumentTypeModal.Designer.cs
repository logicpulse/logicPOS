using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentTypeModal
    {
        public override Size ModalSize => new Size(400, 514);
        public override string ModalTitleResourceName => "window_title_edit_template";

        #region Components
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtAcronym = TextBox.Simple("global_ConfigurationUnitMeasure_Acronym");
        private TextBox _txtPrintCopies = TextBox.Simple("global_print_copies", true, true, RegularExpressions.IntegerNumber);
        private CheckButton _checkRequestPrintConfirmation = new CheckButton(GeneralUtils.GetResourceByName("global_print_request_confirmation"));
        private CheckButton _checkOpenDrawer = new CheckButton(GeneralUtils.GetResourceByName("global_open_drawer"));
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtAcronym.Entry);
            SensitiveFields.Add(_txtPrintCopies.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkRequestPrintConfirmation);
            SensitiveFields.Add(_checkOpenDrawer);
        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_txtDesignation);
            ValidatableFields.Add(_txtPrintCopies);

            if (_modalMode == EntityEditionModalMode.Update)
            {
                ValidatableFields.Add(_txtOrder);
                ValidatableFields.Add(_txtCode);
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }

        private VBox CreateDetailsTab()
        {
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
                detailsTab.PackStart(_txtCode.Component, false, false, 0);
            }

            detailsTab.PackStart(_txtDesignation.Component, false, false, 0);
            detailsTab.PackStart(_txtAcronym.Component, false, false, 0);
            detailsTab.PackStart(_txtPrintCopies.Component, false, false, 0);
            detailsTab.PackStart(_checkRequestPrintConfirmation, false, false, 0);
            detailsTab.PackStart(_checkOpenDrawer, false, false, 0);

            return detailsTab;
        }
    }
}
