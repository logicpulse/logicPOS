using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentSerieModal
    {
        private IconButtonWithText _btnATSeriesComunicate;
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtNextNumber = TextBox.Simple("global_documentfinanceseries_NextDocumentNumber", true, true, @"^[1-9][0-9]*$");
        private TextBox _txtNumberRangeBegin = TextBox.Simple("global_documentfinanceseries_DocumentNumberRangeBegin", true, true, @"^0|[1-9][0-9]*$");
        private TextBox _txtNumberRangeEnd = TextBox.Simple("global_documentfinanceseries_DocumentNumberRangeEnd", true, true, @"^0|[1-9][0-9]*$");
        private TextBox _txtAcronym = TextBox.Simple("global_acronym", true, true, @"^[a-zA-Z0-9\s]{1,30}$");
        private TextBox _txtATDocCodeValidationSerie = TextBox.Simple("global_at_atdoccodeid");
        private EntityComboBox<FiscalYear> _comboFiscalYears;
        private EntityComboBox<DocumentType> _comboDocumentTypes;
        private CheckButton _checkDisabled = new CheckButton(LocalizedString.Instance["global_record_disabled"]);
    }
}
