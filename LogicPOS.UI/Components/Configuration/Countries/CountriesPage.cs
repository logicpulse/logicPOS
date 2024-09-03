using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CountriesPage : Page<Country>
    {
        protected override IRequest<ErrorOr<IEnumerable<Country>>> GetAllQuery => new GetAllCountriesQuery();

        public CountriesPage(Window parent) : base(parent)
        {
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(2));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }

        public override void DeleteEntity()
        {
            throw new System.NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new CountryModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }
    }
}
