using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Currencies.GetAllCurrencies;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CurrenciesPage : Page
    {
        private List<Currency> _currencies = new List<Currency>();
        public CurrenciesPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new System.NotImplementedException();
        }

        protected override void AddColumns()
        {
            var codeColumn = CreateCodeColumn();
            GridView.AppendColumn(codeColumn);

            var designationColumn = CreateDesignationColumn();
            GridView.AppendColumn(designationColumn);

            var acronymColumn = CreateAcronymColumn();
            GridView.AppendColumn(acronymColumn);

            var updateAtColumn = Columns.CreateUpdatedAtColumn(3);
            GridView.AppendColumn(updateAtColumn);
        }

        protected override void AddEntitiesToModel()
        {
            var model = (ListStore)GridViewSettings.Model;
            _currencies.ForEach(country => model.AppendValues(country));
        }

        protected override ListStore CreateGridViewModel()
        {
            return new ListStore(typeof(Currency));
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                search = search.Trim();
                var currency = (Currency)model.GetValue(iterator, 0);

                if (currency.Designation.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting();
            AddDesignationSorting();
            AddAcronymSorting();
            AddLastUpdatedAtSorting();
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

        private void AddLastUpdatedAtSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftCurrency = (Currency)model.GetValue(left, 0);
                var rightCurrency = (Currency)model.GetValue(right, 0);

                if (leftCurrency == null || rightCurrency == null)
                {
                    return 0;
                }

                return leftCurrency.UpdatedAt.CompareTo(rightCurrency.UpdatedAt);
            });
        }

        private void AddDesignationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var leftCurrency = (Currency)model.GetValue(left, 0);
                var rightCurrency = (Currency)model.GetValue(right, 0);

                if (leftCurrency == null || rightCurrency == null)
                {
                    return 0;
                }

                return leftCurrency.Designation.CompareTo(rightCurrency.Designation);
            });
        }

        private void AddCodeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(0, (model, left, right) =>
            {
                var leftCurrency = (Currency)model.GetValue(left, 0);
                var rightCurrency = (Currency)model.GetValue(right, 0);

                if (leftCurrency == null || rightCurrency == null)
                {
                    return 0;
                }

                return leftCurrency.Code.CompareTo(rightCurrency.Code);
            });
        }

        protected override void LoadEntities()
        {
            var currencies = _mediator.Send(new GetAllCurrenciesQuery()).Result;

            if (currencies.IsError)
            {
                ShowApiErrorAlert();
                return;
            }

            _currencies.Clear();
            _currencies.AddRange(currencies.Value);
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var currency = (Currency)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = currency.Designation;
            }

            return Columns.CreateDesignationColumn(RenderDesignation);
        }

        private TreeViewColumn CreateCodeColumn()
        {
            void RenderCode(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var currency = (Currency)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = currency.Code;
            }

            return Columns.CreateCodeColumn(RenderCode);
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


        protected override void RunModal(EntityModalMode mode)
        {
            var modal = new CurrencyModal(mode, SelectedEntity as Currency);
            modal.Run();
            modal.Destroy();
        }
    }
}
