using LogicPOS.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Pages
{
    public partial class FiscalYearsPage
    {
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
