using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.MovementTypes.GetAllMovementTypes;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class MovementTypesPage : Page<MovementType>
    {
        protected override IRequest<ErrorOr<IEnumerable<MovementType>>> GetAllQuery => new GetAllMovementTypesQuery();
        public MovementTypesPage(Window parent) : base(parent)
        {
        }

        public override bool DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new MovementTypeModal(mode, SelectedEntity as MovementType);
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

        #region Singleton
        private static MovementTypesPage _instance;
        public static MovementTypesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MovementTypesPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
