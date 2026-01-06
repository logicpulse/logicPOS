using Gtk;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
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
            if (ThermalPrintingService.WasPrintedByThermalPrinter(SelectedEntity.Id))
            {
                CustomAlerts.Warning()
                             .WithMessage("O documento que tentou imprimir foi Criado em uma impressora Térmica.")
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
                    SelectedDocumentsTotalFinal -= document.TotalFinal;
                }
                else
                {
                    SelectedDocuments.Add(document);
                    SelectedDocumentsTotalFinal += document.TotalFinal;
                }

                PageChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public override void UpdateButtonPrevileges()
        {
            
        }
    }
}
