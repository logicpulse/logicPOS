using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Terminals.GetAllTerminals;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TerminalsPage : Page<Terminal>
    {
        public List<Terminal> SelectedTerminals { get; private set;  } = new List<Terminal>();
        protected override IRequest<ErrorOr<IEnumerable<Terminal>>> GetAllQuery => new GetAllTerminalsQuery();
       
        public TerminalsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            Navigator.BtnInsert.Visible = false;
            Navigator.BtnDelete.Visible = false;

            DisableFilterButton();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new TerminalModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            if (Options != null && Options.Count != 0)
            {
                GridView.AppendColumn(CreateSelectColumn());
                GridView.AppendColumn(Columns.CreateCodeColumn(1));
                GridView.AppendColumn(Columns.CreateDesignationColumn(2));
                GridView.AppendColumn(CreateHardwareIdColumn());
                GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
            }
            else
            {
                GridView.AppendColumn(Columns.CreateCodeColumn(0));
                GridView.AppendColumn(Columns.CreateDesignationColumn(1));
                GridView.AppendColumn(CreateHardwareIdColumn());
                GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
            }
        }
        
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddHardwareIdSorting();
            AddUpdatedAtSorting(3);
        }

        private TreeViewColumn CreateSelectColumn()
        {
            TreeViewColumn selectColumn = new TreeViewColumn();

            var selectCellRenderer = new CellRendererToggle();
            selectColumn.PackStart(selectCellRenderer, true);

            selectCellRenderer.Toggled += CheckBox_Clicked;

            void RenderSelect(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var terminal = (Terminal)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedTerminals.Contains(terminal);
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var terminal = (Terminal)GridView.Model.GetValue(iterator, 0);

                if (SelectedTerminals.Contains(terminal))
                {
                    SelectedTerminals.Remove(terminal);
                }
                else
                {
                    SelectedTerminals.Add(terminal);
                }
            }
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_DELETE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_EDIT");
        }
       
        public static List<Guid> SelectTerminals(Window sourceWindow)
        {
            var page = new TerminalsPage(sourceWindow, PageOptions.SelectionPageOptions);
            var selectTerminalModal = new EntitySelectionModal<Terminal>(page, LocalizedString.Instance["window_title_dialog_select_record"]);
            ResponseType response = (ResponseType)selectTerminalModal.Run();
            var terminalIds = page.SelectedTerminals.Select(t => t.Id).ToList();
            selectTerminalModal.Destroy();
            return terminalIds;
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
