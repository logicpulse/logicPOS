using Gtk;
using System;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using LogicPOS.UI.Components.Modals;

namespace LogicPOS.UI.Components.PoleDisplays
{
    public partial class PoleDisplayModal
    {
        public override Size ModalSize => new Size(500, _modalMode == EntityEditionModalMode.Insert ? 600: 750);
        public override string ModalTitleResourceName => "window_title_edit_configurationpoledisplay";

        #region Components
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtVendorId = TextBox.Simple("global_pole_display_vid",true,true,@"^0x[\d]{4}$");
        private TextBox _txtProductId = TextBox.Simple("global_pole_display_pid", true, true, @"^0x[\d]{4}$");
        private TextBox _txtEndpoint = TextBox.Simple("global_pole_display_endpoint", true, true, @"^Ep[\d]{2}$");
        private TextBox _txtCOMPort = TextBox.Simple("global_pole_display_com_port", true, true, @"^COM[\d]{1}$");
        private TextBox _txtCodeTable = TextBox.Simple("global_pole_display_codetable", true, true, @"^0x[\d]{2}$");
        private TextBox _txtCharsPerLine = TextBox.Simple("global_pole_display_number_of_characters_per_line", true, true, @"^[\d]+$");
        private TextBox _txtStandByInSeconds = TextBox.Simple("global_pole_display_goto_stand_by_in_seconds", true, true, @"^[\d]+$");
        private TextBox _txtStandByLine1 = TextBox.Simple("global_pole_display_stand_by_line_no");
        private TextBox _txtStandByLine2 = TextBox.Simple("global_pole_display_stand_by_line_no");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_txtVendorId.Entry);
            SensitiveFields.Add(_txtProductId.Entry);
            SensitiveFields.Add(_txtEndpoint.Entry);
            SensitiveFields.Add(_txtCOMPort.Entry);
            SensitiveFields.Add(_txtCodeTable.Entry);
            SensitiveFields.Add(_txtCharsPerLine.Entry);
            SensitiveFields.Add(_txtStandByInSeconds.Entry);
            SensitiveFields.Add(_txtStandByLine1.Entry);
            SensitiveFields.Add(_txtStandByLine2.Entry);
        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_txtDesignation);
            ValidatableFields.Add(_txtVendorId);
            ValidatableFields.Add(_txtProductId);
            ValidatableFields.Add(_txtEndpoint);
            ValidatableFields.Add(_txtCOMPort);
            ValidatableFields.Add(_txtCodeTable);
            ValidatableFields.Add(_txtCharsPerLine);
            ValidatableFields.Add(_txtStandByInSeconds);

            if(_modalMode == EntityEditionModalMode.Update)
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
            detailsTab.PackStart(_txtVendorId.Component, false, false, 0);
            detailsTab.PackStart(_txtProductId.Component, false, false, 0);
            detailsTab.PackStart(_txtEndpoint.Component, false, false, 0);
            detailsTab.PackStart(_txtCOMPort.Component, false, false, 0);
            detailsTab.PackStart(_txtCodeTable.Component, false, false, 0);
            detailsTab.PackStart(_txtCharsPerLine.Component, false, false, 0);
            detailsTab.PackStart(_txtStandByInSeconds.Component, false, false, 0);

            var standByLineResource = GeneralUtils.GetResourceByName("global_pole_display_stand_by_line_no");
            _txtStandByLine1.Label.Text = string.Format(standByLineResource, 1);
            detailsTab.PackStart(_txtStandByLine1.Component, false, false, 0);

            _txtStandByLine2.Label.Text = string.Format(standByLineResource, 2);
            detailsTab.PackStart(_txtStandByLine2.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }


    }
}
