using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetDocumentsRelations;
using LogicPOS.Api.Features.Documents.GetDocumentsTotals;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Errors;
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

        private void OnSelectedEntityConfirmed(Document document)
        {
            DocumentPdfUtils.ViewDocumentPdf(SourceWindow, document.Id);
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var document = (Document)GridView.Model.GetValue(iterator, 0);

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

        private void LoadDocumentsTotals()
        {
            var query = new GetDocumentsTotalsQuery(_entities.Select(d => d.Id));
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result,
                                                    source: SourceWindow);
                return;
            }

            if (_totals.Count > 0)
            {
                _totals.Clear();
            }

            _totals.AddRange(result.Value);
        }

        private void LoadDocumentsRelations()
        {
            var query = new GetDocumentsRelationsQuery(_entities.Select(x => x.Id));
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                return;
            }

            if (_relations.Count > 0)
            {
                _relations.Clear();
            }

            _relations.AddRange(result.Value);
        }

        public override void UpdateButtonPrevileges()
        {
            //no implementation needed for this page, this page has no same buttons.  

        }
    }
}
