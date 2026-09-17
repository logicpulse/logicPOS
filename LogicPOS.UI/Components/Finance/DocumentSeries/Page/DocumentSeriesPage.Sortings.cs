using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentSeriesPage
    {
        private void AddFiscalYearSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var leftDocumentSerie = (DocumentSeries)model.GetValue(left, 0);
                var rightDocumentSerie = (DocumentSeries)model.GetValue(right, 0);

                if (leftDocumentSerie == null || rightDocumentSerie == null)
                {
                    return 0;
                }

                return leftDocumentSerie.FiscalYear.Designation.CompareTo(rightDocumentSerie.FiscalYear.Designation);
            });
        }

        private void AddDocumentTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftDocumentSerie = (DocumentSeries)model.GetValue(left, 0);
                var rightDocumentSerie = (DocumentSeries)model.GetValue(right, 0);

                if (leftDocumentSerie == null || rightDocumentSerie == null)
                {
                    return 0;
                }

                return leftDocumentSerie.DocumentType.Designation.CompareTo(rightDocumentSerie.DocumentType.Designation);
            });
        }

    }
}
