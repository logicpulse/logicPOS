using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.Globalization;


namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleSubfamilyModal
    {
        public override Size ModalSize => new Size(500, 460);
        public override string ModalTitleResourceName => "window_title_edit_articlesubfamily";

        #region Components
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtButtonName = TextBox.Simple("global_button_name");
        private ImagePicker _imagePicker = new ImagePicker(LocalizedString.Instance["global_button_image"]);
        private EntityComboBox<CommissionGroup> _comboCommissionGroups;
        private EntityComboBox<ArticleFamily> _comboFamilies;
        private EntityComboBox<DiscountGroup> _comboDiscountGroups;
        private EntityComboBox<VatRate> _comboVatOnTable;
        private EntityComboBox<VatRate> _comboVatDirectSelling;
        private EntityComboBox<Api.Entities.Printer> _comboPrinters;
        #endregion

        private Api.ValueObjects.Button GetButton()
        {
            return new Api.ValueObjects.Button
            {
                Label = _txtButtonName.Text,
                Image = _imagePicker.GetBase64Image(),
                ImageExtension = _imagePicker.GetImageExtension()
            };
        }

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtButtonName.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_imagePicker.Component);
            SensitiveFields.Add(_comboCommissionGroups.Component);
            SensitiveFields.Add(_comboFamilies.Component);
            SensitiveFields.Add(_comboDiscountGroups.Component);
            SensitiveFields.Add(_comboVatOnTable.Component);
            SensitiveFields.Add(_comboVatDirectSelling.Component);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtButtonName);
                    ValidatableFields.Add(_comboFamilies);
                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtButtonName);
                    ValidatableFields.Add(_comboFamilies);
                    break;
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), LocalizedString.Instance["global_record_main_detail"]);
            yield return (CreateDetails2Tab(), LocalizedString.Instance["dialog_edit_articlesubfamily_tab2_label"]);
            yield return (CreateNotesTab(), LocalizedString.Instance["global_notes"]);
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
            detailsTab.PackStart(_txtButtonName.Component, false, false, 0);
            detailsTab.PackStart(_comboFamilies.Component, false, false, 0);
            detailsTab.PackStart(_imagePicker.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }

        private VBox CreateDetails2Tab()
        {
            var details2Tab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            details2Tab.PackStart(_comboCommissionGroups.Component, false, false, 0);
            details2Tab.PackStart(_comboDiscountGroups.Component, false, false, 0);
            details2Tab.PackStart(_comboVatOnTable.Component, false, false, 0);
            details2Tab.PackStart(_comboVatDirectSelling.Component, false, false, 0);
            details2Tab.PackStart(_comboPrinters.Component, false, false, 0);

            return details2Tab;
        }
    }
}
