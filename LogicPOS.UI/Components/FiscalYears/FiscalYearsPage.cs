using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class FiscalYearsPage : Page<FiscalYear>
    {
        protected override IRequest<ErrorOr<IEnumerable<FiscalYear>>> GetAllQuery => new GetAllFiscalYearsQuery();
        public FiscalYearsPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new FiscalYearModal(mode, SelectedEntity as FiscalYear);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(CreateYearColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

      

        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var fiscalYear = (FiscalYear)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = fiscalYear.Acronym;
            }

            var title = GeneralUtils.GetResourceByName("global_acronym");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        private TreeViewColumn CreateYearColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var fiscalYear = (FiscalYear)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = fiscalYear.Year.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_year");
            return Columns.CreateColumn(title, 3, RenderValue);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddYearSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftFiscalYear = (FiscalYear)model.GetValue(left, 0);
                var rightFiscalYear = (FiscalYear)model.GetValue(right, 0);

                if (leftFiscalYear == null || rightFiscalYear == null)
                {
                    return 0;
                }

                return leftFiscalYear.Acronym.CompareTo(rightFiscalYear.Acronym);
            });
        }

        private void AddYearSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftFiscalYear = (FiscalYear)model.GetValue(left, 0);
                var rightFiscalYear = (FiscalYear)model.GetValue(right, 0);

                if (leftFiscalYear == null || rightFiscalYear == null)
                {
                    return 0;
                }

                return leftFiscalYear.Year.CompareTo(rightFiscalYear.Year);
            });
        }
    }
}
