using Gtk;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using OxyPlot.Series;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Agt.RequestSeriesModal
{
    public partial class RequestSeriesModal
    {
        private TextBox _txtEstablishmentNumber = TextBox.Simple("Número do Estabelecimento", true);
        private TextBox _txtYear = TextBox.Simple("global_fiscal_year", true, true, @"^\d+$");
        private EntityComboBox<DocumentType> _comboDocumentTypes;
        private CheckButton _checkContingencyIndicator = new CheckButton("Série de Contingência (N)");

        protected override void Initialize()
        {
            InitializeDocumentTypesComboBox();
            _txtYear.Text = DateTime.Now.Year.ToString();
            _txtEstablishmentNumber.Text = "123";
            _checkContingencyIndicator.Toggled += delegate
            {
                _checkContingencyIndicator.Label = _checkContingencyIndicator.Active ? "Série de Contingência (C)" : "Série de Contingência (N)";
            };
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
