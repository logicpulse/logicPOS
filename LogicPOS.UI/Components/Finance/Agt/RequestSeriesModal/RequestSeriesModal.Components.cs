using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Agt.RequestSeriesModal
{
    public partial class RequestSeriesModal
    {
        private TextBox _txtEstablishmentNumber = TextBox.Simple("Número do Estabelecimento", true);
        private EntityComboBox<DocumentType> _comboDocumentTypes;
        private CheckButton _checkContingencyIndicator = new CheckButton("Série de Contingência (N)");
        private EntityComboBox<FiscalYear> _comboFiscalYears;

        protected override void Initialize()
        {
            InitializeDocumentTypesComboBox();
            InitializeFiscalYearsComboBox();
            _txtEstablishmentNumber.Text = "123";
            _checkContingencyIndicator.Toggled += delegate
            {
                _checkContingencyIndicator.Label = _checkContingencyIndicator.Active ? "Série de Contingência (C)" : "Série de Contingência (N)";
            };
        }

        private void InitializeFiscalYearsComboBox()
        {
            var currentFiscalYear = FiscalYearsService.CurrentFiscalYear;
            var fiscalYears = new List<FiscalYear> { currentFiscalYear };
            var labelText = GeneralUtils.GetResourceByName("global_fiscal_year");
            var defaultFiscalYear =  currentFiscalYear;

            _comboFiscalYears = new EntityComboBox<FiscalYear>(labelText,
                                                             fiscalYears,
                                                             defaultFiscalYear,
                                                             true);

            _comboFiscalYears.ComboBox.Sensitive = false;
        }

        private void InitializeDocumentTypesComboBox()
        {
            string[] eligibleDocTypes = AgtService.EligibleDocumentTypes;
            var documentTypes = DocumentTypesService.GetAll().Where(dc => eligibleDocTypes.Contains(dc.Acronym));
            var labelText = GeneralUtils.GetResourceByName("global_documentfinance_type");

            _comboDocumentTypes = new EntityComboBox<DocumentType>(labelText,
                                                             documentTypes,
                                                             documentTypes.First(),
                                                             true);
        }

    }
}
