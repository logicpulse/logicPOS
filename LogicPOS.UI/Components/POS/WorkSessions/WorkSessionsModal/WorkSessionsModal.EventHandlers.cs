using Gtk;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetAllReportsDataDay;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using Serilog;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WorkSessionsModal
    {
        private void AddButtonsEventHandlers()
        {
            BtnPrintDay.Clicked += BtnPrintDay_Clicked;
         
        }

        
        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Close)
            {
                Run();
            }

            base.OnResponse(response);
        }

        private void BtnPrintDay_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var printer = TerminalService.Terminal.ThermalPrinter;

            if (printer == null)
            {
                CustomAlerts.Warning(this)
                            .WithMessage("Não foi possível encontrar a impressora térmica configurada para o terminal.")
                            .ShowAlert();
                return;
            }
           var result=  _meditaor.Send(new GetAllReportsDataDayQuery(Page.SelectedEntity.Id)).Result;
            
            try
            {
                if(result.IsError)
                {
                    ErrorHandlingService.HandleApiError(result);
                    return;
                }

                var reportData = result.Value;
                foreach (var movement in reportData)
                {
                    ThermalPrintingService.PrintWorkSessionReport(movement);
                        
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error printing document {DocumentId}", Page.SelectedEntity.Id);
                CustomAlerts.Error(this)
                            .WithMessage("Ocorreu um erro ao tentar imprimir o documento.")
                            .ShowAlert();
            }
        }

    }
}
