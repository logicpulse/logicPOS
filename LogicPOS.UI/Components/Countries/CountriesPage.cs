using Gtk;
using LogicPOS.Api.Features.Countries;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CountriesPage : Page
    {
        private List<Country> _countries = new List<Country>();

        public CountriesPage(Window parent) : base(parent)
        {
        }

        protected override void AddEntitiesToModel()
        {
            var model = (ListStore)GridViewSettings.Model;
            _countries.ForEach(country => model.AppendValues(country));
        }

        protected override void AddColumns()
        {
            var codeColumn = CreateCodeColumn();
            GridView.AppendColumn(codeColumn);

            var designationColumn = CreateDesignationColumn();
            GridView.AppendColumn(designationColumn);

            var lastUpdateAtColumn = CreateLastUpdateAtColumn();
            GridView.AppendColumn(lastUpdateAtColumn);
        }

        private TreeViewColumn CreateLastUpdateAtColumn()
        {
            void RenderLastUpdateAt(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var country = (Country)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = country.LastUpdateAt.ToString();
                cell.Xalign = 1;
            }

            var title = GeneralUtils.GetResourceByName("global_record_date_updated");
            var col = Columns.CreateColumn(title, 2, RenderLastUpdateAt);
            col.Alignment = 1;
            return col;
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var country = (Country)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = country.Designation;
            }

            return Columns.CreateDesignationColumn(RenderDesignation);
        }

        private TreeViewColumn CreateCodeColumn()
        {
            void RenderCode(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var country = (Country)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = country.Code;
            }

            return Columns.CreateCodeColumn(RenderCode);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting();
            AddDesignationSorting();
            AddLastUpdatedAtSorting();
        }

        private void AddLastUpdatedAtSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftCountry = (Country)model.GetValue(left, 0);
                var rightCountry = (Country)model.GetValue(right, 0);

                if (leftCountry == null || rightCountry == null)
                {
                    return 0;
                }

                return leftCountry.LastUpdateAt.CompareTo(rightCountry.LastUpdateAt);
            });
        }

        private void AddDesignationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var leftCountry = (Country)model.GetValue(left, 0);
                var rightCountry = (Country)model.GetValue(right, 0);

                if (leftCountry == null || rightCountry == null)
                {
                    return 0;
                }

                return leftCountry.Designation.CompareTo(rightCountry.Designation);
            });
        }

        private void AddCodeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(0, (model, left, right) =>
            {
                var leftCountry = (Country)model.GetValue(left, 0);
                var rightCountry = (Country)model.GetValue(right, 0);

                if (leftCountry == null || rightCountry == null)
                {
                    return 0;
                }

                return leftCountry.Code.CompareTo(rightCountry.Code);
            });
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
                var country = (Country)model.GetValue(iterator, 0);

                if (country.Designation.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        protected override void LoadEntities()
        {
            var countries = _mediator.Send(new GetAllCountriesQuery()).Result;

            if (countries.IsError)
            {
                ShowApiErrorAlert();
                return;
            }

            _countries.Clear();
            _countries.AddRange(countries.Value);
        }

        public override void DeleteEntity()
        {
            throw new System.NotImplementedException();
        }

        public override void InsertEntity()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateEntity()
        {
            throw new System.NotImplementedException();
        }

        public override void ViewEntity()
        {
            throw new System.NotImplementedException();
        }

        protected override ListStore CreateGridViewModel()
        {
            return new ListStore(typeof(Country));
        }

    }
}
