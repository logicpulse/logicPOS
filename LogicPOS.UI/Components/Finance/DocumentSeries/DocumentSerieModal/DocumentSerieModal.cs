using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.Series.AddDocumentSerie;
using LogicPOS.Api.Features.Documents.Series.UpdateDocumentSerie;
using LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentSerieModal : EntityEditionModal<DocumentSeries>
    {
        public DocumentSerieModal(EntityEditionModalMode modalMode, DocumentSeries entity = null) : base(modalMode, entity)
        {
        }

        private IEnumerable<FiscalYear> GetFiscalYears() => ExecuteGetEntitiesQuery(new GetAllFiscalYearsQuery());
        private IEnumerable<DocumentType> GetDocumentTypes() => ExecuteGetEntitiesQuery(new GetAllDocumentTypesQuery());

        private AddDocumentSerieCommand CreateAddCommand()
        {
            return new AddDocumentSerieCommand
            {
                Designation = _txtDesignation.Text,
                NextNumber = int.Parse(_txtNextNumber.Text),
                NumberRangeBegin = int.Parse(_txtNumberRangeBegin.Text),
                NumberRangeEnd = int.Parse(_txtNumberRangeEnd.Text),
                Acronym = _txtAcronym.Text,
                DocumentTypeId = _comboDocumentTypes.SelectedEntity.Id,
                FiscalYearId = _comboFiscalYears.SelectedEntity.Id,
                ATDocCodeValidationSerie = _txtATDocCodeValidationSerie.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateDocumentSerieCommand CreateUpdateCommand()
        {
            return new UpdateDocumentSerieCommand
            {
                Id = _entity.Id,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text
            };
        }

        protected override void ShowEntityData()
        {
            _txtDesignation.Text = _entity.Designation;
            _txtNextNumber.Text = _entity.NextNumber.ToString();
            _txtNumberRangeBegin.Text = _entity.NumberRangeBegin.ToString();
            _txtNumberRangeEnd.Text = _entity.NumberRangeEnd.ToString();
            _txtAcronym.Text = _entity.Acronym;
            _txtATDocCodeValidationSerie.Text = _entity.ATDocCodeValidationSerie;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }
        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

    }
}
