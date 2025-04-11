using Gtk;
using System;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using LogicPOS.UI.Components.Modals;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PoleDisplayModal
    {
        public override Size ModalSize => new Size(500, _modalMode == EntityEditionModalMode.Insert ? 600: 750);
        public override string ModalTitleResourceName => "window_title_edit_configurationpoledisplay";

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
