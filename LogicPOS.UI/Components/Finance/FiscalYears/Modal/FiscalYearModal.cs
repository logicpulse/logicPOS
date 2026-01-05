using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.FiscalYears.CreateFiscalYear;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.DocumentSeries;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
            var relevantData = FiscalYearsService.GetCreationRelevantData();

            if (relevantData.HasValue)
            {
                _txtYear.Text = relevantData.Value.CurrentYear.ToString();
                int currentYearCount = relevantData.Value.CurrentYearCount;
                _txtDesignation.Text = $"Ano {relevantData.Value.CurrentYear} {currentYearCount + 1}";
                _txtAcronym.Text = $"{relevantData.Value.CurrentYear}A{currentYearCount + 1}";
            }
            else
            {
                string currentYear = DateTime.Now.Year.ToString();
                _txtYear.Text = currentYear;
                _txtDesignation.Text = $"Ano {currentYear}";
                _txtAcronym.Text = $"{currentYear}A1";
            }
        }

        private CreateFiscalYearCommand CreateAddCommand()
        {
            return new CreateFiscalYearCommand
            {
                Designation = _txtDesignation.Text,
                Year = DateTime.Now.Year,
                Acronym = _txtAcronym.Text,
                SeriesForEachTerminal = _checkSeriesForEachTerminal.Active,
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
            _checkSeriesForEachTerminal.Active = _entity.SeriesForEachTerminal;
        }

        protected override bool AddEntity()
        {
            if (FiscalYearsService.HasActiveFiscalYear())
            {
                ResponseType dialog1Response = CustomAlerts.Question(BackOfficeWindow.Instance)
                                                           .WithSize(new Size(600, 400))
                                                           .WithTitle(LocalizedString.Instance["window_title_series_fiscal_year_close_current"])
                                                           .WithMessage(string.Format(LocalizedString.Instance["dialog_message_series_fiscal_year_close_current"], FiscalYearsService.CurrentFiscalYear.Designation))
                                                           .ShowAlert();

                if (dialog1Response != ResponseType.Yes)
                {
                    return false;
                }

                if (FiscalYearsService.CloseCurrentFiscalYear() == false)
                {
                    return false;
                }
            }

            var result = _mediator.Send(CreateAddCommand()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            if (SystemInformationService.UseAgtFe)
            {
                return true;
            }

            if (AskForDefaultSeriesCreation())
            {
                List<Guid> terminals = null;
                if (_checkSeriesForEachTerminal.Active)
                {
                    terminals = TerminalsPage.SelectTerminals();
                }

                if (_checkSeriesForEachTerminal.Active && (terminals == null || terminals.Count == 0))
                {
                    CustomAlerts.Warning(this)
                                .WithMessage("O ano fiscal aberto requer séries para cada terminal, mas nenhum terminal foi selecionado: nenhuma série será criada.")
                                .ShowAlert();

                    return true;
                }

                DocumentSeriesService.CreateDefaultSeriesForFiscalYear(FiscalYearsService.CurrentFiscalYear.Id, terminals);
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
