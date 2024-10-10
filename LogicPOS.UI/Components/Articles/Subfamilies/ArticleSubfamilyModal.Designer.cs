using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;


namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleSubfamilyModal
    {
        public override Size ModalSize => new Size(500, 445);
        public override string ModalTitleResourceName => "window_title_edit_articlesubfamily";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private TextBox _txtButtonName = new TextBox("global_button_name");
        private ImagePicker _imagePicker = new ImagePicker(GeneralUtils.GetResourceByName("global_button_image"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private EntityComboBox<CommissionGroup> _comboCommissionGroups;
        private EntityComboBox<ArticleFamily> _comboFamilies;
        private EntityComboBox<DiscountGroup> _comboDiscountGroups;
        private EntityComboBox<VatRate> _comboVatOnTable;
        private EntityComboBox<VatRate> _comboVatDirectSelling;
        #endregion

        protected override void BeforeDesign()
        {
            InitializeCommissionGroupsComboBox();
            InitializeFamiliesComboBox();
            InitializeDiscountGroupsComboBox();
            InitializeVatOnTableComboBox();
            InitializeVatDirectSellingComboBox();
        }

        private void InitializeCommissionGroupsComboBox()
        {
            var groups = GetCommissionGroups();
            var labelText = GeneralUtils.GetResourceByName("global_commission_group");
            var currentCommissionGroup = _entity != null ? _entity.CommissionGroup : null;

            _comboCommissionGroups = new EntityComboBox<CommissionGroup>(labelText,
                                                             groups,
                                                             currentCommissionGroup);
        }

        private void InitializeFamiliesComboBox()
        {
            var families = GetFamilies();
            var labelText = GeneralUtils.GetResourceByName("global_families");
            var currentFamily = _entity != null ? _entity.Family : null;

            _comboFamilies = new EntityComboBox<ArticleFamily>(labelText,
                                                             families,
                                                             currentFamily,
                                                             true);
        }

        private void InitializeDiscountGroupsComboBox()
        {
            var groups = GetDiscountGroups();
            var labelText = GeneralUtils.GetResourceByName("global_discount_group");
            var currentDiscountGroup = _entity != null ? _entity.DiscountGroup : null;

            _comboDiscountGroups = new EntityComboBox<DiscountGroup>(labelText,
                                                             groups,
                                                             currentDiscountGroup);
        }

        private void InitializeVatOnTableComboBox()
        {
            var vatRates = GetVatRates();
            var labelText = GeneralUtils.GetResourceByName("global_vat_on_table");
            var currentVatRate = _entity != null ? _entity.VatOnTable : null;

            _comboVatOnTable = new EntityComboBox<VatRate>(labelText,
                                                             vatRates,
                                                             currentVatRate);
        }

        private void InitializeVatDirectSellingComboBox()
        {
            var vatRates = GetVatRates();
            var labelText = GeneralUtils.GetResourceByName("global_vat_direct_selling");
            var currentVatRate = _entity != null ? _entity.VatDirectSelling : null;

            _comboVatDirectSelling = new EntityComboBox<VatRate>(labelText,
                                                             vatRates,
                                                             currentVatRate);
        }

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
                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtButtonName);
                    break;
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateDetails2Tab(), GeneralUtils.GetResourceByName("dialog_edit_articlesubfamily_tab2_label"));
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

            return details2Tab;
        }
    }
}
