using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentSerieModal
    {
        public override Size ModalSize => new Size(500, 650);
        public override string ModalTitleResourceName => "dialog_edit_DocumentFinanceSeries_tab1_label";

        protected override void Initialize()
        {
            InitializeFiscalYearsComboBox();
            InitializeDocumentTypesComboBox();

            if(_modalMode == EntityEditionModalMode.Insert)
            {
                _txtNextNumber.Text = "1";
                _txtNextNumber.Entry.Sensitive = false;

                _txtNumberRangeBegin.Text = "1";
                _txtNumberRangeBegin.Entry.Sensitive = false;

                _txtNumberRangeEnd.Text = "2147483647";
                _txtNumberRangeEnd.Entry.Sensitive = false;
            }
        }

        private void InitializeFiscalYearsComboBox()
        {
            var currentFiscalYear = FiscalYearsService.CurrentFiscalYear;
            var fiscalYears = new List<FiscalYear> { currentFiscalYear  };
            var labelText = GeneralUtils.GetResourceByName("global_fiscal_year");
            var defaultFiscalYear = _entity != null ? _entity.FiscalYear : currentFiscalYear;

            _comboFiscalYears = new EntityComboBox<FiscalYear>(labelText,
                                                             fiscalYears,
                                                             defaultFiscalYear,
                                                             true);

            _comboFiscalYears.ComboBox.Sensitive = false; 
        }

        private void InitializeDocumentTypesComboBox()
        {
            var documentTypes = DocumentTypesService.GetAll();
            var labelText = GeneralUtils.GetResourceByName("global_documentfinance_type");
            var currentDocumentType = _entity != null ? _entity.DocumentType : null;

            _comboDocumentTypes = new EntityComboBox<DocumentType>(labelText,
                                                             documentTypes,
                                                             currentDocumentType,
                                                             true);

            _comboDocumentTypes.ComboBox.Changed += ComboBox_Changed;
        }

        private void ComboBox_Changed(object sender, System.EventArgs e)
        {
            var selectedFiscalYear = _comboFiscalYears.SelectedEntity;
            var selectedDocType = _comboDocumentTypes.SelectedEntity;

            _txtDesignation.Text = $"{selectedDocType?.Designation} {selectedDocType?.Acronym} {selectedFiscalYear.Acronym}";
            _txtAcronym.Text = $"{selectedDocType?.Acronym} {selectedFiscalYear.Acronym}"; 
        }

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtNextNumber.Entry);
            SensitiveFields.Add(_txtNumberRangeBegin.Entry);
            SensitiveFields.Add(_txtNumberRangeEnd.Entry);
            SensitiveFields.Add(_txtAcronym.Entry);
            SensitiveFields.Add(_txtATDocCodeValidationSerie.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_comboFiscalYears.ComboBox);
            SensitiveFields.Add(_comboDocumentTypes.ComboBox);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtNextNumber);
                    ValidatableFields.Add(_txtNumberRangeBegin);
                    ValidatableFields.Add(_txtNumberRangeEnd);
                    ValidatableFields.Add(_txtAcronym);
                    ValidatableFields.Add(_txtATDocCodeValidationSerie);
                    ValidatableFields.Add(_comboFiscalYears);
                    ValidatableFields.Add(_comboDocumentTypes);
                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtNextNumber);
                    ValidatableFields.Add(_txtNumberRangeBegin);
                    ValidatableFields.Add(_txtNumberRangeEnd);
                    ValidatableFields.Add(_txtAcronym);
                    ValidatableFields.Add(_txtATDocCodeValidationSerie);
                    ValidatableFields.Add(_comboFiscalYears);
                    ValidatableFields.Add(_comboDocumentTypes);
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

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                _comboFiscalYears.ComboBox.Sensitive = false;
                _comboDocumentTypes.ComboBox.Sensitive = false;
                _txtAcronym.Entry.Sensitive = false;
                _txtNextNumber.Entry.Sensitive = false;
                _txtNumberRangeBegin.Entry.Sensitive = false;
                _txtNumberRangeEnd.Entry.Sensitive = false;
                _txtATDocCodeValidationSerie.Entry.Sensitive = false;

            }
            tab1.PackStart(_comboFiscalYears.Component,false, false, 0);
            tab1.PackStart(_comboDocumentTypes.Component, false, false, 0);
            tab1.PackStart(_txtDesignation.Component, false, false, 0);
            tab1.PackStart(_txtNextNumber.Component, false, false, 0);
            tab1.PackStart(_txtNumberRangeBegin.Component, false, false, 0);
            tab1.PackStart(_txtNumberRangeEnd.Component, false, false, 0);
            tab1.PackStart(_txtAcronym.Component, false, false, 0);
            tab1.PackStart(_txtATDocCodeValidationSerie.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }

    }
}
