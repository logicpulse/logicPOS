using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class SizeUnitsPage : Page<SizeUnit>
    {
        public SizeUnitsPage(Window parent) : base(parent)
        {
        }


        protected override IRequest<ErrorOr<IEnumerable<SizeUnit>>> GetAllQuery => new GetAllSizeUnitsQuery();
       
        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new SizeUnitModal(mode, SelectedEntity as SizeUnit);
            modal.Run();
            modal.Destroy();
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

        #region Singleton
        private static SizeUnitsPage _instance;
        public static SizeUnitsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SizeUnitsPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }

}
