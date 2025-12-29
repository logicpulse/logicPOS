using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.Series.UpdateDocumentSerie;
using LogicPOS.Api.Features.Finance.Documents.Series.CreateSeries;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentSerieModal : EntityEditionModal<DocumentSeries>
    {
        public DocumentSerieModal(EntityEditionModalMode modalMode, DocumentSeries entity = null) : base(modalMode, entity)
        {

        }

        private CreateDocumentSeriesCommand CreateAddCommand()
        {
            return new CreateDocumentSeriesCommand
            {
                Designation = _txtDesignation.Text,
                NextNumber = int.Parse(_txtNextNumber.Text),
                NumberRangeBegin = int.Parse(_txtNumberRangeBegin.Text),
                NumberRangeEnd = int.Parse(_txtNumberRangeEnd.Text),
                Acronym = _txtAcronym.Text,
                DocumentTypeId = _comboDocumentTypes.SelectedEntity.Id,
                FiscalYearId = _comboFiscalYears.SelectedEntity.Id,
                AtValidationCode = _txtATDocCodeValidationSerie.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateDocumentSeriesCommand CreateUpdateCommand()
        {
            return new UpdateDocumentSeriesCommand
            {
                Id = _entity.Id,
                Designation = _txtDesignation.Text,
                AtValidationCode = _txtATDocCodeValidationSerie.Text
            };
        }

        protected override void ShowEntityData()
        {
            _txtDesignation.Text = _entity.Designation;
            _txtNextNumber.Text = _entity.NextNumber.ToString();
            _txtNumberRangeBegin.Text = _entity.NumberRangeBegin.ToString();
            _txtNumberRangeEnd.Text = _entity.NumberRangeEnd.ToString();
            _txtAcronym.Text = _entity.Acronym;
            _txtATDocCodeValidationSerie.Text = _entity.ATDocCodeValidationSeries;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

    }
}
