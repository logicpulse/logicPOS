using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Places.DeletePlace;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PlacesPage : Page<Place>
    {
        public PlacesPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<Place>>> GetAllQuery => new GetAllPlacesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PlaceModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreatePriceTypeColumn());
            GridView.AppendColumn(CreateMovementTypeColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddPriceTypeSorting();
            AddMovementTypeSorting();
            AddUpdatedAtSorting(4);
        }
        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeletePlaceCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static PlacesPage _instance;
        public static PlacesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlacesPage(BackOfficeWindow.Instance);
                }

                return _instance;
            }
        }
        #endregion
    }
}
