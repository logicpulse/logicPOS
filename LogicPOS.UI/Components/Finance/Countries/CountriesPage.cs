using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Countries.DeleteCountry;
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

        public CountriesPage(Window parent, Dictionary<string,string> options = null) : base(parent, options)
        {
            DisableFilterButton();
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

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new CountryModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteCountryCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONCOUNTRY_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONCOUNTRY_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONCOUNTRY_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONCOUNTRY_VIEW");

        }

        #region Singleton
        private static CountriesPage _instance;
        public static CountriesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CountriesPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
