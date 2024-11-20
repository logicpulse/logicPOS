using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Terminals.GetAllTerminals;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class TerminalsPage : Page<Terminal>
    {
        protected override IRequest<ErrorOr<IEnumerable<Terminal>>> GetAllQuery => new GetAllTerminalsQuery();
        public TerminalsPage(Window parent) : base(parent)
        {
        }

        public override bool DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new TerminalModal(mode, SelectedEntity as Terminal);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateHardwareIdColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateHardwareIdColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (Terminal)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = terminal.HardwareId;
            }

            var title = GeneralUtils.GetResourceByName("global_hardware_id");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddHardwareIdSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddHardwareIdSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftTerminal = (Terminal)model.GetValue(left, 0);
                var rightTerminal = (Terminal)model.GetValue(right, 0);

                if (leftTerminal == null || rightTerminal == null)
                {
                    return 0;
                }

                return leftTerminal.HardwareId.CompareTo(rightTerminal.HardwareId);
            });
        }

        #region Singleton
        private static TerminalsPage _instance;

        public static TerminalsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TerminalsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }

        #endregion
    }
}
