using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.PriceTypes.DeletePriceType;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class PriceTypesPage : Page<PriceType>
    {
        public PriceTypesPage(Window parent) : base(parent)
        {
            DisableFilterButton();
        }

        protected override IRequest<ErrorOr<IEnumerable<PriceType>>> GetAllQuery => new GetAllPriceTypesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PriceTypeModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
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

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeletePriceTypeCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPRICETYPE_EDIT");
        }
        #region Singleton
        private static PriceTypesPage _instance;
        public static PriceTypesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PriceTypesPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
