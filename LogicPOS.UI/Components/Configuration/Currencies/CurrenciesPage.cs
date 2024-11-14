using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Currencies.GetAllCurrencies;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CurrenciesPage : Page<Currency>
    {
        protected override IRequest<ErrorOr<IEnumerable<Currency>>> GetAllQuery => new GetAllCurrenciesQuery();

        public CurrenciesPage(Window parent, Dictionary<string,string> options = null) : base(parent, options)
        {
        }

        public override void DeleteEntity()
        {
            throw new System.NotImplementedException();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftCurrency = (Currency)model.GetValue(left, 0);
                var rightCurrency = (Currency)model.GetValue(right, 0);

                if (leftCurrency == null || rightCurrency == null)
                {
                    return 0;
                }

                return leftCurrency.Acronym.CompareTo(rightCurrency.Acronym);
            });
        }

        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderAcronym(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var currency = (Currency)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = currency.Acronym;
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationCurrency_Acronym");
            return Columns.CreateColumn(title, 2, RenderAcronym);
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new CurrencyModal(mode, SelectedEntity as Currency);
            modal.Run();
            modal.Destroy();
        }

        #region Singleton
        private static CurrenciesPage _instance;
        public static CurrenciesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CurrenciesPage(null);
                }
                return _instance;
            }
        }
        #endregion

    }
}
