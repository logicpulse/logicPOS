using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public class FiscalYearsPage : Page<FiscalYear>
    {
        protected override IRequest<ErrorOr<IEnumerable<FiscalYear>>> GetAllQuery => new GetAllFiscalYearsQuery();
        public FiscalYearsPage(Window parent) : base(parent)
        {
            Navigator.BtnUpdate.Visible = false;
            Navigator.BtnDelete.Visible = false;
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            if(mode == EntityEditionModalMode.Update)
            {
                mode = EntityEditionModalMode.View;
            }

            if(mode == EntityEditionModalMode.Insert)
            {
                var currentFiscalYear = _entities.FirstOrDefault(f => f.IsDeleted == false);

                if (currentFiscalYear != null)
                {
                    ResponseType dialog1Response = logicpos.Utils.ShowMessageBox(
                        GlobalApp.BackOffice,
                        DialogFlags.Modal,
                        new Size(600, 400),
                        MessageType.Question,
                        ButtonsType.YesNo,
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_series_fiscal_year_close_current"),
                        string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_series_fiscal_year_close_current"), currentFiscalYear.Designation)
                    );

                    if (dialog1Response == ResponseType.No)
                    {
                        return;
                    }
                }
            }
      
            var modal = new FiscalYearModal(modalMode: mode,
                                            entity: SelectedEntity);
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
