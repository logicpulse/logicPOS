using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PoleDisplays;
using LogicPOS.Api.Features.PoleDisplays.GetAllPoleDisplays;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.PoleDisplays
{
    public class PoleDisplaysPage : Page<PoleDisplay>
    {
        public PoleDisplaysPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<PoleDisplay>>> GetAllQuery => new GetAllPoleDisplaysQuery();

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new PoleDisplayModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(4));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(5));
        }

        protected override void InitializeSort()
        {

            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }

        #region Singleton
        private static PoleDisplaysPage _instance;
        public static PoleDisplaysPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PoleDisplaysPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
