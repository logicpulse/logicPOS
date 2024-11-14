using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.FiscalYears.AddFiscalYear;
using LogicPOS.Api.Features.FiscalYears.CreateDefaultSeries;
using LogicPOS.Api.Features.FiscalYears.UpdateFiscalYear;
using LogicPOS.UI.Alerts;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class FiscalYearModal : EntityEditionModal<FiscalYear>
    {
        public FiscalYearModal(EntityEditionModalMode modalMode,
                               FiscalYear entity = null) : base(modalMode, entity)
        {
            _txtYear.Text = DateTime.Now.Year.ToString();
            _txtYear.Entry.Sensitive = false;
        }

        private AddFiscalYearCommand CreateAddCommand()
        {
            return new AddFiscalYearCommand
            {
                Designation = _txtDesignation.Text,
                Year = DateTime.Now.Year,
                Acronym = _txtAcronym.Text,
                SeriesForEachTerminal = _checkSeriesForEachTerminal.Active,
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
                NewSeriesForEachTerminal = _checkSeriesForEachTerminal.Active,
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
            _checkSeriesForEachTerminal.Active = _entity.SeriesForEachTerminal;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void AddEntity()
        {
            var result = _mediator.Send(CreateAddCommand()).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }

            var fiscalYearId = result.Value;

            ResponseType response = CustomAlerts.Question(this)
                                                .WithSize(new Size(600, 400))
                                                .WithTitleResource("window_title_series_create_series")
                                                .WithMessageResource("dialog_message_series_create_document_type_series")
                                                .ShowAlert();

            if (response != ResponseType.Yes)
            {
                return;
            }

            var command = new CreateDefaultSeriesCommand(fiscalYearId);

            ExecuteCommand(command);
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

    }
}
