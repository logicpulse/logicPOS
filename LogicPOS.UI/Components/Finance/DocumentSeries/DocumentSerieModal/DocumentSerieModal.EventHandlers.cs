using LogicPOS.Api.Features.Finance.At.RegisterSeries;
using LogicPOS.UI.Components.Finance.At;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentSerieModal
    {
        private void ComboBox_Changed(object sender, System.EventArgs e)
        {
            var selectedFiscalYear = _comboFiscalYears.SelectedEntity;
            var selectedDocType = _comboDocumentTypes.SelectedEntity;

            _txtDesignation.Text = $"{selectedDocType?.Designation} {selectedDocType?.Acronym} {selectedFiscalYear.Acronym}";
            _txtAcronym.Text = $"{selectedDocType?.Acronym} {selectedFiscalYear.Acronym}";
        }

        private void BtnATSeriesComunicate_Clicked(object sender, System.EventArgs e)
        {
            if(_entity == null || string.IsNullOrWhiteSpace(_txtATDocCodeValidationSerie.Text) == false)
            {
                return;
            }

            var seriesInfo = AtService.RegisterSeries(_entity.Id);

            if(seriesInfo == null)
            {
                return;
            }

            _txtATDocCodeValidationSerie.Text = seriesInfo.Value.ValidationCode;
            _txtNotes.TextView.Buffer.Text = seriesInfo.Value.GetNotes();
            _txtATDocCodeValidationSerie.Entry.Sensitive = false;
            _btnATSeriesComunicate.Visible = false;
        }

    }
}
