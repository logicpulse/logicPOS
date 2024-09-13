using Gtk;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using LogicPOS.Api.Entities;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PlaceModal
    {
        public override Size ModalSize => new Size(500, 450);
        public override string ModalTitleResourceName => "window_title_edit_configurationplacetable";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private EntityComboBox<PriceType> _comboPriceTypes;
        private EntityComboBox<MovementType> _comboMovementTypes;
        #endregion

        protected override void BeforeDesign()
        {
            InitializePriceTypesComboBox();
            InitializeMovementTypesComboBox();
        }

        private void InitializePriceTypesComboBox()
        {
            var priceTypes = GetPriceTypes();
            var labelText = GeneralUtils.GetResourceByName("global_placetable_PriceType");
            var currentPriceType = _entity != null ? _entity.PriceType : null;

            _comboPriceTypes = new EntityComboBox<PriceType>(labelText,
                                                             priceTypes,
                                                             currentPriceType,
                                                             true);
        }

        private void InitializeMovementTypesComboBox()
        {
            var movementTypes = GetMovementTypes();
            var labelText = GeneralUtils.GetResourceByName("global_placetable_MovementType");
            var currentMovementType = _entity != null ? _entity.MovementType : null;

            _comboMovementTypes = new EntityComboBox<MovementType>(labelText,
                                                             movementTypes,
                                                             currentMovementType,
                                                             true);
        }

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_comboPriceTypes.ComboBox);
            SensitiveFields.Add(_comboMovementTypes.ComboBox);
        }

        protected override void AddValidatableFields()
        {
            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_comboPriceTypes);
                    ValidatableFields.Add(_comboMovementTypes);
                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_comboPriceTypes);
                    ValidatableFields.Add(_comboMovementTypes);
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
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
                detailsTab.PackStart(_txtCode.Component, false, false, 0);
            }

            detailsTab.PackStart(_txtDesignation.Component, false, false, 0);
            detailsTab.PackStart(_comboPriceTypes.Component, false, false, 0);
            detailsTab.PackStart(_comboMovementTypes.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }
    }
}
