using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.WorkSessions.GetAllClosedDays;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class WorkSessionsPage : Page<WorkSessionPeriod>
    {
        public event EventHandler PageChanged;
        protected override IRequest<ErrorOr<IEnumerable<WorkSessionPeriod>>> GetAllQuery => new GetAllClosedDaysQuery();

        public WorkSessionsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            DisableFilterButton();
        }

        public override int RunModal(EntityEditionModalMode mode) => (int)ResponseType.None;

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateSelectColumn());
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateStartDateColumn());
            GridView.AppendColumn(CreateEndDateColumn());
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddDesignationSorting(1);
        }

       
        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

        public override void UpdateButtonPrevileges()
        {
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var day = (WorkSessionPeriod)GridView.Model.GetValue(iterator, 0);

                SelectedEntity = day;

                PageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #region Singleton
        private static WorkSessionsPage _instance;
        public static WorkSessionsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WorkSessionsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
