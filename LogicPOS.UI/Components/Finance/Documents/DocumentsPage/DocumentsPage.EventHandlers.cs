using Gtk;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using LogicPOS.Utility;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        public event EventHandler PageChanged;

        private void AddEventHandlers()
        {
            SelectedEntityConfirmed += OnSelectedEntityConfirmed;
        }

        private void OnSelectedEntityConfirmed(DocumentViewModel document)
        {
            if (ThermalPrintingService.DocumentWasPrintedByThermalPrinter(SelectedEntity.Id))
            {
                var message = string.Format(GeneralUtils.GetResourceByName("window_dialog_cant_open_document"), SelectedEntity.Number);
                CustomAlerts.Warning()
                             .WithMessage(message)
                             .ShowAlert();
                return;

            }
            DocumentPdfUtils.ViewDocumentPdf(SourceWindow, document.Id);
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var document = (DocumentViewModel)GridView.Model.GetValue(iterator, 0);

                if (SelectedDocuments.Contains(document))
                {
                    SelectedDocuments.Remove(document);
                }
                else
                {
                    SelectedDocuments.Add(document);
                }

                SelectedDocumentsTotalFinal = CalculateSelectedDocumentsTotalFinal();

                PageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private decimal CalculateSelectedDocumentsTotalFinal()
        {
            decimal invoicesTotalFinal  = SelectedDocuments.Where(d => d.Type != "NC").Sum(d => d.TotalFinal);
            decimal creditNotesTotalFinal = SelectedDocuments.Where(d => d.Type == "NC").Sum(d => d.TotalFinal);
            decimal totalFinal = invoicesTotalFinal - creditNotesTotalFinal;
            return totalFinal < 0 ? totalFinal*(-1) : totalFinal;
        }

        public override void UpdateButtonPrevileges()
        {
            
        }
    }
}
