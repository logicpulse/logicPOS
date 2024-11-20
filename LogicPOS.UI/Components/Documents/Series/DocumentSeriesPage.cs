using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class DocumentSeriesPage : Page<DocumentSeries>
    {
        protected override IRequest<ErrorOr<IEnumerable<DocumentSeries>>> GetAllQuery => new GetAllDocumentSeriesQuery();
        public DocumentSeriesPage(Window parent) : base(parent)
        {
            Navigator.BtnInsert.Visible = false;
            Navigator.BtnDelete.Visible = false;
        }

        public override bool DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (mode == EntityEditionModalMode.Update)
            {
                mode = EntityEditionModalMode.View;
            }

            var modal = new DocumentSerieModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(CreateFiscalYearColumn());
            GridView.AppendColumn(CreateDocumentTypeColumn());
            GridView.AppendColumn(Columns.CreateDesignationColumn(3));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

        private TreeViewColumn CreateFiscalYearColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (DocumentSeries)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.FiscalYear.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_year");
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateDocumentTypeColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (DocumentSeries)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.DocumentType.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_documentfinance_type");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddFiscalYearSorting();
            AddDocumentTypeSorting();
            AddDesignationSorting(3);
            AddUpdatedAtSorting(4);
        }

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

        #region Singleton
        private static DocumentSeriesPage _instance;
        public static DocumentSeriesPage Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new DocumentSeriesPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
