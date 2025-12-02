using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.FiscalYears.AddFiscalYear;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.DocumentSeries;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

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

        protected override bool AddEntity()
        {
            var result = _mediator.Send(CreateAddCommand()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            if(SystemInformationService.SystemInformation.IsAngola && AppSettings.Instance.UseAgtFe)
            {
                //Don't create document type series automatically in Angola with AGT FE
                return true;
            }

            if (AskForDefaultSeriesCreation())
            {
                DocumentSeriesService.CreateDefaultSeriesForFiscalYear(FiscalYearsService.CurrentFiscalYear.Id);
            }

            return true;
        }

        private bool AskForDefaultSeriesCreation()
        {
            string messageRosource = SystemInformationService.SystemInformation.IsPortugal ?
                "dialog_message_series_create_document_type_series_pt" :
                "dialog_message_series_create_document_type_series";
            ResponseType responseType = CustomAlerts.Question(this)
                .WithTitleResource("window_title_series_create_series")
                .WithMessageResource(messageRosource)
                .WithSize(new Size(600, 400))
                .ShowAlert();
            return responseType == ResponseType.Yes;
        }

        protected override bool UpdateEntity() => false;

    }
}
