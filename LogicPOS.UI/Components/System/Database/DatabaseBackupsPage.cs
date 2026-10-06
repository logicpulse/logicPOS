using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Database;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    /// <summary>
    /// Read-only list of the backups recorded by the API, used to choose which one to restore.
    /// </summary>
    public class DatabaseBackupsPage : Page<DatabaseBackup>
    {
        protected override IRequest<ErrorOr<IEnumerable<DatabaseBackup>>> GetAllQuery => new GetDatabaseBackupsQuery();

        public DatabaseBackupsPage(Window parent) : base(parent, PageOptions.SelectionPageOptions)
        {
            DisableCommonFilterButtons();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateVersionColumn());
            GridView.AppendColumn(CreateCreatedAtColumn());
            GridView.AppendColumn(CreateFileNameColumn());
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                var backup = model.GetValue(iterator, 0) as DatabaseBackup;
                return string.IsNullOrWhiteSpace(search) || (backup?.FileName?.ToLower().Contains(search) ?? false);
            };
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddSorting(0, backup => backup.Version);
            AddSorting(1, backup => backup.CreatedAt);
            AddSorting(2, backup => backup.FileName);
        }

        private void AddSorting(int sortColumnId, Func<DatabaseBackup, IComparable> key)
        {
            GridViewSettings.Sort.SetSortFunc(sortColumnId, (model, left, right) =>
            {
                var leftBackup = (DatabaseBackup)model.GetValue(left, 0);
                var rightBackup = (DatabaseBackup)model.GetValue(right, 0);

                if (leftBackup == null || rightBackup == null)
                {
                    return 0;
                }

                return Comparer<IComparable>.Default.Compare(key(leftBackup), key(rightBackup));
            });
        }

        private TreeViewColumn CreateVersionColumn()
        {
            void RenderVersion(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var backup = (DatabaseBackup)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = backup.Version.ToString();
            }

            var versionColumn = Columns.CreateColumn(LocalizedString.Instance["global_version"], 0, RenderVersion);
            versionColumn.MinWidth = 80;
            return versionColumn;
        }

        private TreeViewColumn CreateCreatedAtColumn()
        {
            void RenderCreatedAt(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var backup = (DatabaseBackup)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = backup.CreatedAt.ToString();
            }

            var createdAtColumn = Columns.CreateColumn(LocalizedString.Instance["global_date"], 1, RenderCreatedAt);
            createdAtColumn.MinWidth = 180;
            return createdAtColumn;
        }

        private TreeViewColumn CreateFileNameColumn()
        {
            void RenderFileName(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var backup = (DatabaseBackup)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = backup.FileName;
            }

            var fileNameColumn = Columns.CreateColumn(LocalizedString.Instance["global_file"], 2, RenderFileName);
            fileNameColumn.Expand = true;
            return fileNameColumn;
        }

        public override int RunModal(EntityEditionModalMode mode) => (int)ResponseType.None;

        protected override DeleteCommand GetDeleteCommand() => null;

        public override void UpdateButtonPrevileges()
        {
        }
    }
}
