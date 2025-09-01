using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;


namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleFamilyModal
    {
        public override Size ModalSize => new Size(500, 550);
        public override string ModalTitleResourceName => "global_article_class";

        

        protected override void BeforeDesign()
        {
            InitializeCommissionGroupsComboBox();
            InitializePrintersComboBox();
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

        private void InitializeCommissionGroupsComboBox()
        {
            var groups = GetCommissionGroups();
            var labelText = GeneralUtils.GetResourceByName("global_commission_group");
            var currentCommissionGroup = _entity != null ? _entity.CommissionGroup : null;

            _comboCommissionGroups = new EntityComboBox<CommissionGroup>(labelText,
                                                             groups,
                                                             currentCommissionGroup);
        }

        private void InitializePrintersComboBox()
        {
            var printers = GetPrinters();
            var labelText = GeneralUtils.GetResourceByName("global_printers");
            if (_entity != null)
            {
                var currentPrinter = PrinterAssociationService.GetPrinter(_entity.Id);

                _comboPrinters = new EntityComboBox<Api.Entities.Printer>(labelText,
                                                                          printers,
                                                                          currentPrinter,
                                                                          false);
            }
            else
            {
                               _comboPrinters = new EntityComboBox<Api.Entities.Printer>(labelText,
                                                                                          printers,
                                                                                          null,
                                                                                          false);
            }
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
            detailsTab.PackStart(_imagePicker.Component, false, false, 0);
            detailsTab.PackStart(_comboCommissionGroups.Component, false, false, 0);
            detailsTab.PackStart(_comboPrinters.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }
    }
}
