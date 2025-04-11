using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class FiscalYearsPage : Page<FiscalYear>
    {
        protected override IRequest<ErrorOr<IEnumerable<FiscalYear>>> GetAllQuery => new GetAllFiscalYearsQuery();

        public FiscalYearsPage(Window parent) : base(parent)
        {
            Navigator.BtnUpdate.Visible = false;
            Navigator.BtnDelete.Visible = false;
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (mode == EntityEditionModalMode.Update)
            {
                mode = EntityEditionModalMode.View;
            }

            if (mode == EntityEditionModalMode.Insert)
            {
                var currentFiscalYear = _entities.FirstOrDefault(f => f.IsDeleted == false);

                if (currentFiscalYear != null)
                {
                    ResponseType dialog1Response = CustomAlerts.Question(BackOfficeWindow.Instance)
                                                               .WithSize(new Size(600, 400))
                                                               .WithTitle(LocalizedString.Instance["window_title_series_fiscal_year_close_current"])
                                                               .WithMessage(string.Format(LocalizedString.Instance["dialog_message_series_fiscal_year_close_current"], currentFiscalYear.Designation))
                                                               .ShowAlert();

                    if (dialog1Response == ResponseType.No)
                    {
                        return (int)ResponseType.No;
                    }
                }
            }

            var modal = new FiscalYearModal(modalMode: mode,
                                            entity: SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
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

        protected override DeleteCommand GetDeleteCommand() => null;

    }
}
