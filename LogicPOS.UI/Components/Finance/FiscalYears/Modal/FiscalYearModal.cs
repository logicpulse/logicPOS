using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.FiscalYears.AddFiscalYear;
using LogicPOS.Api.Features.FiscalYears.UpdateFiscalYear;
using LogicPOS.UI.Errors;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class FiscalYearModal : EntityEditionModal<FiscalYear>
    {
        public FiscalYearModal(EntityEditionModalMode modalMode,
                               FiscalYear entity = null) : base(modalMode, entity)
        {
            if (modalMode == EntityEditionModalMode.Insert)
            {
                InitializeForInsert();
            }
            _txtYear.Entry.Sensitive = false;
        }

        private void InitializeForInsert()
        {
            string currentYear = DateTime.Now.Year.ToString();
            _txtYear.Text = currentYear;
            _txtDesignation.Text = $"Ano {currentYear}";
            _txtAcronym.Text = $"{currentYear}A1";
        }

        private AddFiscalYearCommand CreateAddCommand()
        {
            return new AddFiscalYearCommand
            {
                Designation = _txtDesignation.Text,
                Year = DateTime.Now.Year,
                Acronym = _txtAcronym.Text,
                SeriesForEachTerminal = false,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateFiscalYearCommand CreateUpdateCommand()
        {
            return new UpdateFiscalYearCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewAcronym = _txtAcronym.Text,
                NewSeriesForEachTerminal = false,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtYear.Text = _entity.Year.ToString();
            _txtAcronym.Text = _entity.Acronym;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void AddEntity()
        {
            var result = _mediator.Send(CreateAddCommand()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

    }
}
