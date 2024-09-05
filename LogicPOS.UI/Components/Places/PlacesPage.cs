using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class PlacesPage : Page<Place>
    {
        public PlacesPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<Place>>> GetAllQuery => new GetAllPlacesQuery();

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new PlaceModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreatePriceTypeColumn());
            GridView.AppendColumn(CreateMovementTypeColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

        private TreeViewColumn CreatePriceTypeColumn()
        {
            void RenderPriceType(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var place = (Place)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = place.PriceType.Designation.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPlace_PriceType");
            return Columns.CreateColumn(title, 2, RenderPriceType);
        }

        private TreeViewColumn CreateMovementTypeColumn()
        {
            void RenderMovementType(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var place = (Place)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = place.MovementType.Designation.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPlace_MovementType");
            return Columns.CreateColumn(title,3, RenderMovementType);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }
    }
}
